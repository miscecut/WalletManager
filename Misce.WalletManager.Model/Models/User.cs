using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.Model.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
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
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDateTime { get; set; }
    }
}
