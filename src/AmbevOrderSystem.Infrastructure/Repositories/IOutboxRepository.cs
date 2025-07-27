using AmbevOrderSystem.Infrastructure.Entities;

namespace AmbevOrderSystem.Infrastructure.Repositories
{
    public interface IOutboxRepository
    {
        Task<OutboxMessage> AddAsync(OutboxMessage message);
        Task<OutboxMessage> UpdateAsync(OutboxMessage message);
        Task<List<OutboxMessage>> GetPendingMessagesAsync(int batchSize = 10);
        Task<List<OutboxMessage>> GetRetryMessagesAsync(int batchSize = 10);
        Task<List<OutboxMessage>> GetMessagesByTypeAsync(string type, int batchSize = 10);
        Task<List<OutboxMessage>> GetMessagesByCorrelationIdAsync(string correlationId);
        Task<int> GetPendingCountAsync();
        Task DeleteCompletedMessagesAsync(DateTime olderThan);
    }
}