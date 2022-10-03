using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.Model.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public User User { get; set; } = null!;
        [StringLength(50)]
        public string? Title { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public Account? FromAccount { get; set; }
        public Account? ToAccount { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public TransactionSubCategory? SubCategory { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
