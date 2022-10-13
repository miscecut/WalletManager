using Misce.WalletManager.DTO.CustomValidationRules;
using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountCreationDTOIn
    {
        [Required(ErrorMessage = "The account type ID must be provided")]
        public Guid? AccountTypeId { get; init; }
        [Required(ErrorMessage = "The account initial amount must be provided")]
        [Currency]
        public decimal? InitialAmount { get; init; }
        [MaxLength(500, ErrorMessage = "The account description is too long")]
        public string? Description { get; init; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The account name must be provided")]
        [MaxLength(100, ErrorMessage = "The account name is too long")]
        public string Name { get; init; } = null!;
        [Required(ErrorMessage = "The account status must be provided")]
        public bool? IsActive { get; init; }
    }
}
