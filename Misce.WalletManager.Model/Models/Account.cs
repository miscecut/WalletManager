using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.Model.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public virtual User User { get; set; } = null!;
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        public virtual AccountType AccountType { get; set; } = null!;
        [Required]
        public decimal InitialAmount { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
