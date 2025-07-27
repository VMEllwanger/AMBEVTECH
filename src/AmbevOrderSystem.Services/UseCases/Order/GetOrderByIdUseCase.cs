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
    /// UseCase para buscar pedido por ID
    /// </summary>
    public class GetOrderByIdUseCase : IUseCase<GetOrderByIdCommand, GetOrderResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetOrderByIdUseCase> _logger;

        public GetOrderByIdUseCase(
            IOrderRepository orderRepository,
            ILogger<GetOrderByIdUseCase> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Result<GetOrderResponse>> ExecuteAsync(GetOrderByIdCommand command)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(command.OrderId);
                if (order == null)
                {
                    return Result<GetOrderResponse>.Failure("Pedido não encontrado");
                }

                var response = OrderMapper.ToGetResponse(order);

                return Result<GetOrderResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido por ID: {OrderId}", command.OrderId);
                return Result<GetOrderResponse>.Failure("Erro interno ao buscar pedido");
            }
        }
    }
}