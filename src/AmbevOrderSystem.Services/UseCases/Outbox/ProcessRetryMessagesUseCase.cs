using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.Models.Responses.Outbox;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Outbox
{
    /// <summary>
    /// UseCase para processar mensagens de retry no Outbox
    /// </summary>
    public class ProcessRetryMessagesUseCase : IUseCase<ProcessRetryMessagesCommand, ProcessMessagesResponse>
    {
        private readonly IOutboxService _outboxService;
        private readonly ILogger<ProcessRetryMessagesUseCase> _logger;

        public ProcessRetryMessagesUseCase(
            IOutboxService outboxService,
            ILogger<ProcessRetryMessagesUseCase> logger)
        {
            _outboxService = outboxService;
            _logger = logger;
        }

        public async Task<Result<ProcessMessagesResponse>> ExecuteAsync(ProcessRetryMessagesCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase ProcessRetryMessages para comando {CommandId}", command.CommandId);

                await _outboxService.ProcessRetryMessagesAsync();

                var response = new ProcessMessagesResponse
                {
                    Message = "Processamento de mensagens de retry iniciado"
                };

                _logger.LogInformation("Processamento de mensagens de retry iniciado com sucesso");
                return Result<ProcessMessagesResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase ProcessRetryMessages para comando {CommandId}", command.CommandId);
                return Result<ProcessMessagesResponse>.Failure("Erro interno ao processar mensagens de retry");
            }
        }
    }
}