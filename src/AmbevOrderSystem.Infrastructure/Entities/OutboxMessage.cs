using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    /// <summary>
    /// Entidade para implementar o Outbox Pattern
    /// Garante que mensagens/pedidos não sejam perdidos mesmo em falhas
    /// </summary>
    public class OutboxMessage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Data { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = OutboxMessageStatus.Pending;

        public int RetryCount { get; set; } = 0;

        public int MaxRetries { get; set; } = 3;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public string? ErrorMessage { get; set; }

        [StringLength(100)]
        public string? CorrelationId { get; set; }
    }

    public static class OutboxMessageStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
        public const string Retry = "Retry";
    }

    public static class OutboxMessageType
    {
        public const string AmbevOrderSubmission = "AmbevOrderSubmission";
    }
}