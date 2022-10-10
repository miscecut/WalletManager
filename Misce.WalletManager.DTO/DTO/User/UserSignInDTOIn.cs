using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.User
{
    public record UserSignInDTOIn
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The username has to be provided")]
        public string Username { get; init; } = null!;
        [Required(ErrorMessage = "The password has to be provided")]
        [MinLength(8, ErrorMessage = "The password has to be at least 8 characters long")]
        public string Password { get; init; } = null!;
    }
}
