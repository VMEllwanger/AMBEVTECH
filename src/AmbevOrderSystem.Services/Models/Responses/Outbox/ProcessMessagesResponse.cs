namespace AmbevOrderSystem.Services.Models.Responses.Outbox
{
    /// <summary>
    /// Resposta para processamento de mensagens no Outbox
    /// </summary>
    public class ProcessMessagesResponse
    {
        public string Message { get; set; } = string.Empty;
        public string ResponseId { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}