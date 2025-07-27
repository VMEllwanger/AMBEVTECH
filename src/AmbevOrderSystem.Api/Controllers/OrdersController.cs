using AmbevOrderSystem.Api.Controllers.Base;
using AmbevOrderSystem.Api.Mappers;
using AmbevOrderSystem.Api.Resquet;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Responses.Order;
using Microsoft.AspNetCore.Mvc;

namespace AmbevOrderSystem.Api.Controllers
{
    [ApiController]
    [Route("api/resellers/{resellerId}/[controller]")]
    public class OrdersController : HelperController
    {
        private readonly IUseCase<CreateCustomerOrderCommand, CreateCustomerOrderResponse> _createOrderUseCase;
        private readonly IUseCase<GetOrderByIdCommand, GetOrderResponse> _getOrderByIdUseCase;
        private readonly IUseCase<GetOrdersByResellerCommand, GetAllOrdersResponse> _getOrdersByResellerUseCase;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(
            IUseCase<CreateCustomerOrderCommand, CreateCustomerOrderResponse> createOrderUseCase,
            IUseCase<GetOrderByIdCommand, GetOrderResponse> getOrderByIdUseCase,
            IUseCase<GetOrdersByResellerCommand, GetAllOrdersResponse> getOrdersByResellerUseCase,
            ILogger<OrdersController> logger)
        {
            _createOrderUseCase = createOrderUseCase;
            _getOrderByIdUseCase = getOrderByIdUseCase;
            _getOrdersByResellerUseCase = getOrdersByResellerUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Criar pedido do cliente
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerOrder(int resellerId, [FromBody] CreateCustomerOrderRequest request)
        {
            var command = RequestMapper.ToCommand(request, resellerId);

            var result = await _createOrderUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                return GenerateErrorResponse(result);
            }

            return CreatedAtAction(nameof(GetOrder), new { resellerId, orderId = result.Data!.OrderId }, result.Data);
        }

        /// <summary>
        /// Obter pedido por ID
        /// </summary>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int resellerId, int orderId)
        {
            var command = new GetOrderByIdCommand
            {
                OrderId = orderId
            };

            var result = await _getOrderByIdUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                return GenerateErrorResponse(result);
            }

            if (result.Data!.ResellerId != resellerId)
            {
                return NotFound(new { message = "Pedido não pertence à revenda especificada" });
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Listar pedidos da revenda
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetOrdersByReseller(int resellerId)
        {
            var command = new GetOrdersByResellerCommand
            {
                ResellerId = resellerId
            };

            var result = await _getOrdersByResellerUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                return GenerateErrorResponse(result);
            }

            return Ok(result.Data);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : HelperController
    {
        private readonly IUseCase<ProcessPendingOrdersCommand, ProcessPendingOrdersResponse> _processPendingOrdersUseCase;
        private readonly ILogger<SystemController> _logger;

        public SystemController(
            IUseCase<ProcessPendingOrdersCommand, ProcessPendingOrdersResponse> processPendingOrdersUseCase,
            ILogger<SystemController> logger)
        {
            _processPendingOrdersUseCase = processPendingOrdersUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Processar pedidos pendentes
        /// </summary>
        [HttpPost("process-pending-orders")]
        public async Task<IActionResult> ProcessPendingOrders()
        {
            var command = new ProcessPendingOrdersCommand();

            var result = await _processPendingOrdersUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                return GenerateErrorResponse(result);
            }

            return Ok(new
            {
                message = "Pedidos pendentes processados com sucesso",
                processedOrders = result.Data!.ProcessedOrders,
                sentToAmbev = result.Data!.SentToAmbev,
                failedOrders = result.Data!.FailedOrders
            });
        }

        /// <summary>
        /// Health check
        /// </summary>
        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}