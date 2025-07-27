using AmbevOrderSystem.Infrastructure.DTOs;
using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AmbevOrderSystem.Services.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IResellerRepository _resellerRepository;
        private readonly IAmbevApiService _ambevApiService;
        private readonly ILogger<OutboxService> _logger;

        public OutboxService(
            IOutboxRepository outboxRepository,
            IOrderRepository orderRepository,
            IResellerRepository resellerRepository,
            IAmbevApiService ambevApiService,
            ILogger<OutboxService> logger)
        {
            _outboxRepository = outboxRepository;
            _orderRepository = orderRepository;
            _resellerRepository = resellerRepository;
            _ambevApiService = ambevApiService;
            _logger = logger;
        }

        public async Task<OutboxMessage> EnqueueMessageAsync(string type, object data, string? correlationId = null)
        {
            var message = new OutboxMessage
            {
                Type = type,
                Data = JsonSerializer.Serialize(data),
                Status = OutboxMessageStatus.Pending,
                CorrelationId = correlationId ?? Guid.NewGuid().ToString()
            };

            return await _outboxRepository.AddAsync(message);
        }

        public async Task<OutboxMessage> EnqueueAmbevOrderAsync(int resellerId, List<int> orderIds, string? correlationId = null)
        {
            var data = new
            {
                ResellerId = resellerId,
                OrderIds = orderIds,
                CreatedAt = DateTime.UtcNow
            };

            return await EnqueueMessageAsync(OutboxMessageType.AmbevOrderSubmission, data, correlationId);
        }

        public async Task ProcessPendingMessagesAsync()
        {
            var pendingMessages = await _outboxRepository.GetPendingMessagesAsync(10);

            foreach (var message in pendingMessages)
            {
                await ProcessMessageAsync(message);
            }
        }

        public async Task ProcessRetryMessagesAsync()
        {
            var retryMessages = await _outboxRepository.GetRetryMessagesAsync(10);

            foreach (var message in retryMessages)
            {
                await ProcessMessageAsync(message);
            }
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await _outboxRepository.GetPendingCountAsync();
        }

        public async Task<List<OutboxMessage>> GetMessagesByCorrelationIdAsync(string correlationId)
        {
            return await _outboxRepository.GetMessagesByCorrelationIdAsync(correlationId);
        }

        private async Task ProcessMessageAsync(OutboxMessage message)
        {
            try
            {
                _logger.LogInformation("Processando mensagem do outbox. ID: {Id}, Tipo: {Type}",
                    message.Id, message.Type);

                message.Status = OutboxMessageStatus.Processing;
                await _outboxRepository.UpdateAsync(message);

                switch (message.Type)
                {
                    case OutboxMessageType.AmbevOrderSubmission:
                        await ProcessAmbevOrderSubmissionAsync(message);
                        break;
                    default:
                        throw new NotSupportedException($"Tipo de mensagem não suportado: {message.Type}");
                }

                message.Status = OutboxMessageStatus.Completed;
                message.ProcessedAt = DateTime.UtcNow;
                await _outboxRepository.UpdateAsync(message);

                _logger.LogInformation("Mensagem processada com sucesso. ID: {Id}", message.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem do outbox. ID: {Id}", message.Id);

                message.RetryCount++;
                message.ErrorMessage = ex.Message;

                if (message.RetryCount >= message.MaxRetries)
                {
                    message.Status = OutboxMessageStatus.Failed;
                    _logger.LogError("Mensagem falhou definitivamente após {RetryCount} tentativas. ID: {Id}",
                        message.RetryCount, message.Id);
                }
                else
                {
                    message.Status = OutboxMessageStatus.Retry;
                    message.NextRetryAt = DateTime.UtcNow.AddMinutes(CalculateRetryDelay(message.RetryCount));
                    _logger.LogWarning("Mensagem agendada para retry em {NextRetryAt}. ID: {Id}, Tentativa: {RetryCount}",
                        message.NextRetryAt, message.Id, message.RetryCount);
                }

                await _outboxRepository.UpdateAsync(message);
            }
        }

        private async Task ProcessAmbevOrderSubmissionAsync(OutboxMessage message)
        {
            var data = JsonSerializer.Deserialize<JsonElement>(message.Data);
            var resellerId = data.GetProperty("ResellerId").GetInt32();
            var orderIds = data.GetProperty("OrderIds").EnumerateArray().Select(x => x.GetInt32()).ToList();

            var orders = await _orderRepository.GetByIdsAsync(orderIds);
            if (!orders.Any())
            {
                throw new InvalidOperationException($"Pedidos não encontrados: {string.Join(", ", orderIds)}");
            }

            var reseller = await _resellerRepository.GetByIdAsync(resellerId);
            if (reseller == null)
            {
                throw new InvalidOperationException($"Revenda não encontrada: {resellerId}");
            }

            var totalQuantity = orders.SelectMany(o => o.Items).Sum(i => i.Quantity);
            if (totalQuantity < 1000)
            {
                throw new InvalidOperationException($"Quantidade total {totalQuantity} não atingiu mínimo de 1000 unidades");
            }

            var allItems = orders.SelectMany(o => o.Items)
                .GroupBy(i => i.ProductSku)
                .Select(g => new OrderItemDto
                {
                    ProductSku = g.Key,
                    ProductName = g.First().ProductName,
                    Quantity = g.Sum(i => i.Quantity),
                    UnitPrice = g.First().UnitPrice
                }).ToList();

            var ambevRequest = new AmbevOrderRequest
            {
                ResellerCnpj = reseller.Cnpj,
                Items = allItems
            };

            _logger.LogInformation("Enviando pedido para Ambev via Outbox. Revenda: {ResellerId}, Pedidos: {OrderIds}, Total: {Quantity}",
                resellerId, string.Join(", ", orderIds), totalQuantity);

            var ambevResponse = await _ambevApiService.SubmitOrderAsync(ambevRequest);

            foreach (var order in orders)
            {
                order.Status = OrderStatus.SentToAmbev;
                order.AmbevOrderNumber = ambevResponse.OrderNumber;
                await _orderRepository.UpdateAsync(order);
            }

            _logger.LogInformation("Pedido enviado com sucesso para Ambev via Outbox. Número: {OrderNumber}",
                ambevResponse.OrderNumber);
        }

        public int CalculateRetryDelay(int retryCount)
        {

            return Math.Min((int)Math.Pow(2, retryCount), 16);
        }
    }
}