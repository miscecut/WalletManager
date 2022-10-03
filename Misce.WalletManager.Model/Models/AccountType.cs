using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.Model.Models
{
    public class AccountType
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
