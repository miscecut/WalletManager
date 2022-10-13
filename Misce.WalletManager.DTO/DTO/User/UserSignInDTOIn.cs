using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.User
{
    public record UserSignInDTOIn
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The username must be provided")]
        public string Username { get; init; } = null!;
        [Required(ErrorMessage = "The password must be provided")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "The password must be at least 8 characters long, contain an uppercase and a lowercase letter, and a number ")]
        public string Password { get; init; } = null!;
        [Required(ErrorMessage = "The password must be confirmed")]
        [Compare(nameof(Password), ErrorMessage = "The passwords don't match")]
        public string ConfirmPassword { get; init; } = null!;
    }
}
