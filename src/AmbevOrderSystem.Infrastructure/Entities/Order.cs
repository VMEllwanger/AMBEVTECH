using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    public class CustomerOrder
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerIdentification { get; set; } = string.Empty;

        public int ResellerId { get; set; }
        public Reseller Reseller { get; set; } = null!;

        public List<OrderItem> Items { get; set; } = new();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? AmbevOrderNumber { get; set; }
    }
}