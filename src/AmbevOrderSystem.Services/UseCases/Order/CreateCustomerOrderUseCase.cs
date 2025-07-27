using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Mappers;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Responses.Order;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;
using AmbevOrderRequest = AmbevOrderSystem.Infrastructure.DTOs.AmbevOrderRequest;

namespace AmbevOrderSystem.Services.UseCases.Order
{
    /// <summary>
    /// UseCase para criação de pedido de cliente
    /// </summary>
    public class CreateCustomerOrderUseCase : IUseCase<CreateCustomerOrderCommand, CreateCustomerOrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IResellerRepository _resellerRepository;
        private readonly IOrderFactory _orderFactory;
        private readonly IOutboxService _outboxService;
        private readonly ILogger<CreateCustomerOrderUseCase> _logger;

        public CreateCustomerOrderUseCase(
            IOrderRepository orderRepository,
            IResellerRepository resellerRepository,
            IOrderFactory orderFactory,
            IOutboxService outboxService,
            ILogger<CreateCustomerOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
            _resellerRepository = resellerRepository;
            _orderFactory = orderFactory;
            _outboxService = outboxService;
            _logger = logger;
        }

        public async Task<Result<CreateCustomerOrderResponse>> ExecuteAsync(CreateCustomerOrderCommand command)
        {
            try
            {
                _logger.LogInformation("Criando pedido do cliente {Customer} para revenda {ResellerId}",
                    command.CustomerIdentification, command.ResellerId);

                var reseller = await _resellerRepository.GetByIdAsync(command.ResellerId);
                if (reseller == null)
                {
                    return Result<CreateCustomerOrderResponse>.Failure("Revenda não encontrada");
                }

                if (string.IsNullOrWhiteSpace(command.CustomerIdentification))
                    return Result<CreateCustomerOrderResponse>.Failure("Identificação do cliente é obrigatória");

                if (!command.Items.Any())
                    return Result<CreateCustomerOrderResponse>.Failure("Deve haver pelo menos um item no pedido");

                var request = new DTOs.CreateCustomerOrderRequest
                {
                    CustomerIdentification = command.CustomerIdentification,
                    Items = command.Items.Select(i => new DTOs.OrderItemDto
                    {
                        ProductSku = i.ProductSku,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };

                var order = await _orderFactory.CreateAsync(command.ResellerId, request);
                var createdOrder = await _orderRepository.AddAsync(order);

                var (correlationId, enqueued) = await TrySubmitToAmbevViaOutboxAsync(command.ResellerId);

                var response = OrderMapper.ToCreateResponse(createdOrder);
                response.CorrelationId = correlationId;
                response.EnqueuedForProcessing = enqueued;

                _logger.LogInformation("Pedido criado com sucesso. ID: {OrderId}", createdOrder.Id);
                return Result<CreateCustomerOrderResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                return Result<CreateCustomerOrderResponse>.Failure("Erro interno ao criar pedido");
            }
        }

        private async Task<(string? correlationId, bool enqueued)> TrySubmitToAmbevViaOutboxAsync(int resellerId)
        {
            try
            {
                var totalQuantity = await _orderRepository.GetTotalQuantityByResellerIdAsync(resellerId);

                if (totalQuantity < 1000)
                {
                    _logger.LogInformation("Quantidade total {Quantity} não atingiu mínimo de 1000 unidades para revenda {ResellerId}",
                        totalQuantity, resellerId);
                    return (null, false);
                }

                var pendingOrders = await _orderRepository.GetPendingOrdersByResellerIdAsync(resellerId);

                if (!pendingOrders.Any())
                {
                    _logger.LogInformation("Nenhum pedido pendente para revenda {ResellerId}", resellerId);
                    return (null, false);
                }

                var orderIds = pendingOrders.Select(o => o.Id).ToList();
                var correlationId = Guid.NewGuid().ToString();

                _logger.LogInformation("Enfileirando pedido para Ambev via Outbox. Revenda: {ResellerId}, Pedidos: {OrderIds}, Total: {Quantity} unidades",
                    resellerId, string.Join(", ", orderIds), totalQuantity);

                await _outboxService.EnqueueAmbevOrderAsync(resellerId, orderIds, correlationId);

                _logger.LogInformation("Pedido enfileirado com sucesso no Outbox. CorrelationId: {CorrelationId}", correlationId);

                return (correlationId, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enfileirar pedido no Outbox. Revenda: {ResellerId}", resellerId);
                return (null, false);
            }
        }
    }
}