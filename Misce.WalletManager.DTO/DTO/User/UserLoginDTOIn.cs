using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.User
{
    public record UserLoginDTOIn
    {
        [Required(ErrorMessage = "The username must be provided")]
        public string Username { get; init; } = null!;
        [Required(ErrorMessage = "The password must be provided")]
        public string Password { get; init; } = null!;
    }
}
