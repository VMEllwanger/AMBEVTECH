using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.Models.Responses.Outbox;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Outbox
{
    /// <summary>
    /// UseCase para buscar mensagens por CorrelationId
    /// </summary>
    public class GetMessagesByCorrelationIdUseCase : IUseCase<GetMessagesByCorrelationIdCommand, GetMessagesByCorrelationIdResponse>
    {
        private readonly IOutboxService _outboxService;
        private readonly ILogger<GetMessagesByCorrelationIdUseCase> _logger;

        public GetMessagesByCorrelationIdUseCase(
            IOutboxService outboxService,
            ILogger<GetMessagesByCorrelationIdUseCase> logger)
        {
            _outboxService = outboxService;
            _logger = logger;
        }

        public async Task<Result<GetMessagesByCorrelationIdResponse>> ExecuteAsync(GetMessagesByCorrelationIdCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase GetMessagesByCorrelationId para CorrelationId: {CorrelationId}", command.CorrelationId);

                var messages = await _outboxService.GetMessagesByCorrelationIdAsync(command.CorrelationId);

                var response = new GetMessagesByCorrelationIdResponse
                {
                    Messages = messages
                };

                _logger.LogInformation("Encontradas {Count} mensagens para CorrelationId: {CorrelationId}", messages.Count, command.CorrelationId);
                return Result<GetMessagesByCorrelationIdResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase GetMessagesByCorrelationId para CorrelationId: {CorrelationId}", command.CorrelationId);
                return Result<GetMessagesByCorrelationIdResponse>.Failure("Erro interno ao obter mensagens");
            }
        }
    }
}