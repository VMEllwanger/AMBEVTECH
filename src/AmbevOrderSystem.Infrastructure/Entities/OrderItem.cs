using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductSku { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 999999.99)]
        public decimal UnitPrice { get; set; }

        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;
    }
}