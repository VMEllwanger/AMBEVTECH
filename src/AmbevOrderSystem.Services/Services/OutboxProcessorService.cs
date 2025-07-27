using AmbevOrderSystem.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.Services
{
    /// <summary>
    /// Serviço de background para processar mensagens do Outbox
    /// Garante que pedidos sejam enviados para a Ambev mesmo em caso de falhas
    /// </summary>
    public class OutboxProcessorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _retryInterval = TimeSpan.FromMinutes(2);

        public OutboxProcessorService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxProcessorService iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

                    await outboxService.ProcessPendingMessagesAsync();

                    await outboxService.ProcessRetryMessagesAsync();

                    await Task.Delay(_processingInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("OutboxProcessorService cancelado");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no OutboxProcessorService");
                    await Task.Delay(_retryInterval, stoppingToken);
                }
            }

            _logger.LogInformation("OutboxProcessorService finalizado");
        }
    }
}