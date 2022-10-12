using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountCreationDTOIn
    {
        [Required(ErrorMessage = "The account type ID has to be provided")]
        public Guid AccountTypeId { get; init; }
        [Required(ErrorMessage = "The account initial amount has to be provided")]
        public decimal InitialAmount { get; init; }
        [MaxLength(500, ErrorMessage = "The account description is too long")]
        public string? Description { get; init; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The account name has to be provided")]
        [MaxLength(100, ErrorMessage = "The account name is too long")]
        public string Name { get; init; } = null!;
        [Required(ErrorMessage = "The account status has to be provided")]
        public bool IsActive { get; init; }
    }
}
