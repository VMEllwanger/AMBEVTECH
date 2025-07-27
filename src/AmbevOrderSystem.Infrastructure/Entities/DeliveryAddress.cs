using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    public class DeliveryAddress
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Number { get; set; } = string.Empty;

        [StringLength(100)]
        public string Complement { get; set; } = string.Empty;

        [StringLength(100)]
        public string Neighborhood { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public int ResellerId { get; set; }
        public Reseller Reseller { get; set; } = null!;
    }
}