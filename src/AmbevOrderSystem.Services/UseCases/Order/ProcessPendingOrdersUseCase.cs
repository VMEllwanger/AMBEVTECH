using AmbevOrderSystem.Infrastructure.DTOs;
using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Responses.Order;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Order
{
    /// <summary>
    /// UseCase para processar pedidos pendentes
    /// </summary>
    public class ProcessPendingOrdersUseCase : IUseCase<ProcessPendingOrdersCommand, ProcessPendingOrdersResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IResellerRepository _resellerRepository;
        private readonly IAmbevApiService _ambevApiService;
        private readonly ILogger<ProcessPendingOrdersUseCase> _logger;

        public ProcessPendingOrdersUseCase(
            IOrderRepository orderRepository,
            IResellerRepository resellerRepository,
            IAmbevApiService ambevApiService,
            ILogger<ProcessPendingOrdersUseCase> logger)
        {
            _orderRepository = orderRepository;
            _resellerRepository = resellerRepository;
            _ambevApiService = ambevApiService;
            _logger = logger;
        }

        public async Task<Result<ProcessPendingOrdersResponse>> ExecuteAsync(ProcessPendingOrdersCommand command)
        {
            try
            {
                _logger.LogInformation("Processando pedidos pendentes");

                var resellers = await _resellerRepository.GetAllAsync();
                var response = new ProcessPendingOrdersResponse();

                foreach (var reseller in resellers)
                {
                    var result = await ProcessResellerOrdersAsync(reseller.Id);
                    response.ProcessedOrders += result.ProcessedOrders;
                    response.SentToAmbev += result.SentToAmbev;
                    response.FailedOrders += result.FailedOrders;
                }

                return Result<ProcessPendingOrdersResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pedidos pendentes");
                return Result<ProcessPendingOrdersResponse>.Failure("Erro interno ao processar pedidos");
            }
        }

        private async Task<ProcessPendingOrdersResponse> ProcessResellerOrdersAsync(int resellerId)
        {
            var response = new ProcessPendingOrdersResponse();
            var pendingOrders = await _orderRepository.GetPendingOrdersAsync();
            var resellerOrders = pendingOrders.Where(o => o.ResellerId == resellerId).ToList();

            if (!resellerOrders.Any())
                return response;

            response.ProcessedOrders = resellerOrders.Count;
            var totalQuantity = resellerOrders.SelectMany(o => o.Items).Sum(i => i.Quantity);

            if (totalQuantity < 1000)
            {
                _logger.LogInformation("Quantidade total {Quantity} não atingiu mínimo de 1000 unidades para revenda {ResellerId}",
                    totalQuantity, resellerId);
                return response;
            }

            var reseller = resellerOrders.First().Reseller;
            var allItems = resellerOrders.SelectMany(o => o.Items)
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

            try
            {
                _logger.LogInformation("Enviando pedido para Ambev. Revenda: {ResellerId}, Total: {Quantity} unidades",
                    resellerId, totalQuantity);

                var ambevResponse = await _ambevApiService.SubmitOrderAsync(ambevRequest);

                foreach (var order in resellerOrders)
                {
                    order.Status = OrderStatus.SentToAmbev;
                    order.AmbevOrderNumber = ambevResponse.OrderNumber;
                    await _orderRepository.UpdateAsync(order);
                }

                response.SentToAmbev = resellerOrders.Count;
                _logger.LogInformation("Pedido enviado com sucesso para Ambev. Número: {OrderNumber}",
                    ambevResponse.OrderNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar pedido para Ambev. Revenda: {ResellerId}", resellerId);

                foreach (var order in resellerOrders)
                {
                    order.Status = OrderStatus.Retry;
                    await _orderRepository.UpdateAsync(order);
                }

                response.FailedOrders = resellerOrders.Count;
            }

            return response;
        }
    }
}