using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.Models.Responses.Outbox;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Outbox
{
    /// <summary>
    /// UseCase para obter contagem de mensagens pendentes no Outbox
    /// </summary>
    public class GetPendingCountUseCase : IUseCase<GetPendingCountCommand, GetPendingCountResponse>
    {
        private readonly IOutboxService _outboxService;
        private readonly ILogger<GetPendingCountUseCase> _logger;

        public GetPendingCountUseCase(
            IOutboxService outboxService,
            ILogger<GetPendingCountUseCase> logger)
        {
            _outboxService = outboxService;
            _logger = logger;
        }

        public async Task<Result<GetPendingCountResponse>> ExecuteAsync(GetPendingCountCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase GetPendingCount para comando {CommandId}", command.CommandId);

                var count = await _outboxService.GetPendingCountAsync();

                var response = new GetPendingCountResponse
                {
                    PendingCount = count
                };

                _logger.LogInformation("Contagem de mensagens pendentes obtida: {Count}", count);
                return Result<GetPendingCountResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase GetPendingCount para comando {CommandId}", command.CommandId);
                return Result<GetPendingCountResponse>.Failure("Erro interno ao obter contagem de mensagens pendentes");
            }
        }
    }
}