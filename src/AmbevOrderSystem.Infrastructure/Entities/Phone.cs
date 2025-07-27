using System.ComponentModel.DataAnnotations;

namespace AmbevOrderSystem.Infrastructure.Entities
{
    public class Phone
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Number { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }
        public int ResellerId { get; set; }
        public Reseller Reseller { get; set; } = null!;
    }
}