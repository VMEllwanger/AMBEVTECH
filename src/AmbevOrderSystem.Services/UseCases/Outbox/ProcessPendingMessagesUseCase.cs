using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.Models.Responses.Outbox;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Outbox
{
    /// <summary>
    /// UseCase para processar mensagens pendentes no Outbox
    /// </summary>
    public class ProcessPendingMessagesUseCase : IUseCase<ProcessPendingMessagesCommand, ProcessMessagesResponse>
    {
        private readonly IOutboxService _outboxService;
        private readonly ILogger<ProcessPendingMessagesUseCase> _logger;

        public ProcessPendingMessagesUseCase(
            IOutboxService outboxService,
            ILogger<ProcessPendingMessagesUseCase> logger)
        {
            _outboxService = outboxService;
            _logger = logger;
        }

        public async Task<Result<ProcessMessagesResponse>> ExecuteAsync(ProcessPendingMessagesCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase ProcessPendingMessages para comando {CommandId}", command.CommandId);

                await _outboxService.ProcessPendingMessagesAsync();

                var response = new ProcessMessagesResponse
                {
                    Message = "Processamento de mensagens pendentes iniciado"
                };

                _logger.LogInformation("Processamento de mensagens pendentes iniciado com sucesso");
                return Result<ProcessMessagesResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase ProcessPendingMessages para comando {CommandId}", command.CommandId);
                return Result<ProcessMessagesResponse>.Failure("Erro interno ao processar mensagens pendentes");
            }
        }
    }
}