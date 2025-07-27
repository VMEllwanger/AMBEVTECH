using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    public class Reseller
    {
        public int Id { get; set; }

        [Required]
        [StringLength(14)]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string RazaoSocial { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public List<Phone> Phones { get; set; } = new();
        public List<Contact> Contacts { get; set; } = new();
        public List<DeliveryAddress> DeliveryAddresses { get; set; } = new();
        public List<CustomerOrder> Orders { get; set; } = new();

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}