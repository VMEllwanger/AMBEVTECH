using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Result;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Mappers;
using Microsoft.Extensions.Logging;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Responses.Order;

namespace AmbevOrderSystem.Services.UseCases.Order
{
    /// <summary>
    /// UseCase para buscar pedidos por revenda
    /// </summary>
    public class GetOrdersByResellerUseCase : IUseCase<GetOrdersByResellerCommand, GetAllOrdersResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrdersByResellerUseCase> _logger;

        public GetOrdersByResellerUseCase(
            IOrderRepository orderRepository,
            ILogger<GetOrdersByResellerUseCase> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<GetAllOrdersResponse>> ExecuteAsync(GetOrdersByResellerCommand command)
        {
            try
            {
                var orders = await _orderRepository.GetByResellerIdAsync(command.ResellerId);

                var response = new GetAllOrdersResponse
                {
                    Orders = OrderMapper.ToGetResponseList(orders)
                };

                return Result<GetAllOrdersResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos da revenda: {ResellerId}", command.ResellerId);
                return Result<GetAllOrdersResponse>.Failure("Erro interno ao buscar pedidos");
            }
        }
    }
}