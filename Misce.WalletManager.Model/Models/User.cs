using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Misce.WalletManager.Model.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = null!;
        [Required]
        [StringLength(500)]
        public string Password { get; set; } = null!;
        [Required]
        [StringLength(500)]
        public string Salt { get; set; } = null!;
        public DateTime? LastLoginDateTime { get; set; }
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDateTime { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
