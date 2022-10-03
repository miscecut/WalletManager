using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.Model.Models
{
    public class TransactionSubCategory
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public TransactionCategory Category { get; set; } = null!;
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
