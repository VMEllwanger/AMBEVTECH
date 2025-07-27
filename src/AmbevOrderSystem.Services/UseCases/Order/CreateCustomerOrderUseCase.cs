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
        private readonly IAmbevApiService _ambevApiService;
        private readonly ILogger<CreateCustomerOrderUseCase> _logger;

        public CreateCustomerOrderUseCase(
            IOrderRepository orderRepository,
            IResellerRepository resellerRepository,
            IOrderFactory orderFactory,
            IAmbevApiService ambevApiService,
            ILogger<CreateCustomerOrderUseCase> logger)
        {
            _orderRepository = orderRepository;
            _resellerRepository = resellerRepository;
            _orderFactory = orderFactory;
            _ambevApiService = ambevApiService;
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

                await TrySubmitToAmbevAsync(command.ResellerId);

                var response = OrderMapper.ToCreateResponse(createdOrder);

                _logger.LogInformation("Pedido criado com sucesso. ID: {OrderId}", createdOrder.Id);
                return Result<CreateCustomerOrderResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido");
                return Result<CreateCustomerOrderResponse>.Failure("Erro interno ao criar pedido");
            }
        }

        private async Task TrySubmitToAmbevAsync(int resellerId)
        {
            try
            {
                var allOrders = await _orderRepository.GetByResellerIdAsync(resellerId);
                var pendingOrders = allOrders.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Retry).ToList();

                if (!pendingOrders.Any())
                {
                    _logger.LogInformation("Nenhum pedido pendente para revenda {ResellerId}", resellerId);
                    return;
                }

                var totalQuantity = pendingOrders.SelectMany(o => o.Items).Sum(i => i.Quantity);

                if (totalQuantity < 1000)
                {
                    _logger.LogInformation("Quantidade total {Quantity} não atingiu mínimo de 1000 unidades para revenda {ResellerId}",
                        totalQuantity, resellerId);
                    return;
                }

                var reseller = pendingOrders.First().Reseller;
                var allItems = pendingOrders.SelectMany(o => o.Items)
                    .GroupBy(i => i.ProductSku)
                    .Select(g => new AmbevOrderSystem.Infrastructure.DTOs.OrderItemDto
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

                _logger.LogInformation("Enviando pedido para Ambev. Revenda: {ResellerId}, Total: {Quantity} unidades",
                    resellerId, totalQuantity);

                var ambevResponse = await _ambevApiService.SubmitOrderAsync(ambevRequest);

                foreach (var order in pendingOrders)
                {
                    order.Status = OrderStatus.SentToAmbev;
                    order.AmbevOrderNumber = ambevResponse.OrderNumber;
                    await _orderRepository.UpdateAsync(order);
                }

                _logger.LogInformation("Pedido enviado com sucesso para Ambev. Número: {OrderNumber}",
                    ambevResponse.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar pedido para Ambev. Revenda: {ResellerId}", resellerId);

                var allOrders = await _orderRepository.GetByResellerIdAsync(resellerId);
                var pendingOrders = allOrders.Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Retry).ToList();
                foreach (var order in pendingOrders)
                {
                    order.Status = OrderStatus.Retry;
                    await _orderRepository.UpdateAsync(order);
                }
            }
        }
    }
}