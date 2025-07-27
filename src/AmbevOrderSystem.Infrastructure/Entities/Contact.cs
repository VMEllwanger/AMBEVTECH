using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public int ResellerId { get; set; }
        public Reseller Reseller { get; set; } = null!;
    }
}