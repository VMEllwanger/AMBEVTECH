using AmbevOrderSystem.Infrastructure.Entities;

namespace AmbevOrderSystem.Services.Interfaces
{
    public interface IOutboxService
    {
        Task<OutboxMessage> EnqueueMessageAsync(string type, object data, string? correlationId = null);
        Task<OutboxMessage> EnqueueAmbevOrderAsync(int resellerId, List<int> orderIds, string? correlationId = null);
        Task ProcessPendingMessagesAsync();
        Task ProcessRetryMessagesAsync();
        Task<int> GetPendingCountAsync();
        Task<List<OutboxMessage>> GetMessagesByCorrelationIdAsync(string correlationId);
    }
}