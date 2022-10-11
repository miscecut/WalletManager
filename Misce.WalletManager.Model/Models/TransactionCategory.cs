using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.Model.Models
{
    public class TransactionCategory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public bool IsExpenseCategory { get; set; }
        [Required]
        public User User { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
