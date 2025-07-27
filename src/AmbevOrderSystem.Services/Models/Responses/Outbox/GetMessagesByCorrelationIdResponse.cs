using AmbevOrderSystem.Infrastructure.Entities;

namespace AmbevOrderSystem.Services.Models.Responses.Outbox
{
    /// <summary>
    /// Resposta para mensagens por CorrelationId
    /// </summary>
    public class GetMessagesByCorrelationIdResponse
    {
        public List<OutboxMessage> Messages { get; set; } = new();
        public string ResponseId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}