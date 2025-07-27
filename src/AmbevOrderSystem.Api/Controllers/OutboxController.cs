using AmbevOrderSystem.Api.Controllers.Base;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.Models.Responses.Outbox;
using Microsoft.AspNetCore.Mvc;

namespace AmbevOrderSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OutboxController : HelperController
    {
        private readonly IUseCase<GetPendingCountCommand, GetPendingCountResponse> _getPendingCountUseCase;
        private readonly IUseCase<GetMessagesByCorrelationIdCommand, GetMessagesByCorrelationIdResponse> _getMessagesByCorrelationIdUseCase;
        private readonly IUseCase<ProcessPendingMessagesCommand, ProcessMessagesResponse> _processPendingMessagesUseCase;
        private readonly IUseCase<ProcessRetryMessagesCommand, ProcessMessagesResponse> _processRetryMessagesUseCase;
        private readonly ILogger<OutboxController> _logger;

        public OutboxController(
            IUseCase<GetPendingCountCommand, GetPendingCountResponse> getPendingCountUseCase,
            IUseCase<GetMessagesByCorrelationIdCommand, GetMessagesByCorrelationIdResponse> getMessagesByCorrelationIdUseCase,
            IUseCase<ProcessPendingMessagesCommand, ProcessMessagesResponse> processPendingMessagesUseCase,
            IUseCase<ProcessRetryMessagesCommand, ProcessMessagesResponse> processRetryMessagesUseCase,
            ILogger<OutboxController> logger)
        {
            _getPendingCountUseCase = getPendingCountUseCase;
            _getMessagesByCorrelationIdUseCase = getMessagesByCorrelationIdUseCase;
            _processPendingMessagesUseCase = processPendingMessagesUseCase;
            _processRetryMessagesUseCase = processRetryMessagesUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Obtém o número de mensagens pendentes no Outbox
        /// </summary>
        [HttpGet("pending-count")]
        public async Task<IActionResult> GetPendingCount()
        {
            _logger.LogInformation("Recebida requisição para obter contagem de mensagens pendentes");

            var command = new GetPendingCountCommand();
            var result = await _getPendingCountUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao obter contagem de mensagens pendentes: {Error}", result.ErrorMessage);
                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Contagem de mensagens pendentes obtida com sucesso: {Count}", result.Data!.PendingCount);
            return Ok(new { PendingCount = result.Data.PendingCount });
        }

        /// <summary>
        /// Obtém mensagens por CorrelationId
        /// </summary>
        [HttpGet("messages/{correlationId}")]
        public async Task<IActionResult> GetMessagesByCorrelationId(string correlationId)
        {
            _logger.LogInformation("Recebida requisição para obter mensagens por CorrelationId: {CorrelationId}", correlationId);

            var command = new GetMessagesByCorrelationIdCommand(correlationId);
            var result = await _getMessagesByCorrelationIdUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao obter mensagens por CorrelationId {CorrelationId}: {Error}", correlationId, result.ErrorMessage);
                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Mensagens obtidas com sucesso para CorrelationId: {CorrelationId}, Count: {Count}",
                correlationId, result.Data!.Messages.Count);
            return Ok(result.Data.Messages);
        }

        /// <summary>
        /// Força o processamento de mensagens pendentes
        /// </summary>
        [HttpPost("process-pending")]
        public async Task<IActionResult> ProcessPendingMessages()
        {
            _logger.LogInformation("Recebida requisição para processar mensagens pendentes");

            var command = new ProcessPendingMessagesCommand();
            var result = await _processPendingMessagesUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao processar mensagens pendentes: {Error}", result.ErrorMessage);
                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Processamento de mensagens pendentes iniciado com sucesso");
            return Ok(new { Message = result.Data!.Message });
        }

        /// <summary>
        /// Força o processamento de mensagens de retry
        /// </summary>
        [HttpPost("process-retry")]
        public async Task<IActionResult> ProcessRetryMessages()
        {
            _logger.LogInformation("Recebida requisição para processar mensagens de retry");

            var command = new ProcessRetryMessagesCommand();
            var result = await _processRetryMessagesUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao processar mensagens de retry: {Error}", result.ErrorMessage);
                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Processamento de mensagens de retry iniciado com sucesso");
            return Ok(new { Message = result.Data!.Message });
        }
    }
}