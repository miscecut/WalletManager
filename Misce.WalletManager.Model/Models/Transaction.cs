using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Misce.WalletManager.Model.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public virtual User User { get; set; } = null!;
        [StringLength(50)]
        public string? Title { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public virtual Account? FromAccount { get; set; }
        public virtual Account? ToAccount { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public virtual TransactionSubCategory? SubCategory { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDateTime { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
