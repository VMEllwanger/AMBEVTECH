using AmbevOrderSystem.Infrastructure.Data;
using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Infrastructure.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OutboxRepository> _logger;

        public OutboxRepository(AppDbContext context, ILogger<OutboxRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OutboxMessage> AddAsync(OutboxMessage message)
        {
            _logger.LogDebug("Adicionando mensagem ao outbox. Tipo: {Type}, CorrelationId: {CorrelationId}",
                message.Type, message.CorrelationId);

            _context.OutboxMessages.Add(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<OutboxMessage> UpdateAsync(OutboxMessage message)
        {
            _logger.LogDebug("Atualizando mensagem do outbox. ID: {Id}, Status: {Status}",
                message.Id, message.Status);

            _context.OutboxMessages.Update(message);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 10)
        {
            return await _context.OutboxMessages
                .Where(x => x.Status == OutboxMessageStatus.Pending)
                .OrderBy(x => x.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<List<OutboxMessage>> GetRetryMessagesAsync(int batchSize = 10)
        {
            var now = DateTime.UtcNow;
            return await _context.OutboxMessages
                .Where(x => x.Status == OutboxMessageStatus.Retry &&
                           x.NextRetryAt <= now &&
                           x.RetryCount < x.MaxRetries)
                .OrderBy(x => x.NextRetryAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<List<OutboxMessage>> GetMessagesByTypeAsync(string type, int batchSize = 10)
        {
            return await _context.OutboxMessages
                .Where(x => x.Type == type &&
                           (x.Status == OutboxMessageStatus.Pending ||
                            (x.Status == OutboxMessageStatus.Retry && x.NextRetryAt <= DateTime.UtcNow)))
                .OrderBy(x => x.CreatedAt)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<List<OutboxMessage>> GetMessagesByCorrelationIdAsync(string correlationId)
        {
            return await _context.OutboxMessages
                .Where(x => x.CorrelationId == correlationId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetPendingCountAsync()
        {
            return await _context.OutboxMessages
                .CountAsync(x => x.Status == OutboxMessageStatus.Pending ||
                                (x.Status == OutboxMessageStatus.Retry && x.NextRetryAt <= DateTime.UtcNow));
        }

        public async Task DeleteCompletedMessagesAsync(DateTime olderThan)
        {
            var messagesToDelete = await _context.OutboxMessages
                .Where(x => x.Status == OutboxMessageStatus.Completed &&
                           x.ProcessedAt < olderThan)
                .ToListAsync();

            if (messagesToDelete.Any())
            {
                _context.OutboxMessages.RemoveRange(messagesToDelete);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Removidas {Count} mensagens completadas do outbox mais antigas que {OlderThan}",
                    messagesToDelete.Count, olderThan);
            }
        }
    }
}