namespace AmbevOrderSystem.Services.Models.Responses.Outbox
{
    /// <summary>
    /// Resposta para contagem de mensagens pendentes no Outbox
    /// </summary>
    public class GetPendingCountResponse
    {
        public int PendingCount { get; set; }
        public string ResponseId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}