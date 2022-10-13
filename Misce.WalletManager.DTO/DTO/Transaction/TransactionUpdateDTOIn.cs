using Misce.WalletManager.DTO.CustomValidationRules;
using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.Transaction
{
    public record TransactionUpdateDTOIn
    {
        [MaxLength(50, ErrorMessage = "The transaction title is too long")]
        public string? Title { get; init; }
        [MaxLength(500, ErrorMessage = "The transaction description is too long")]
        public string? Description { get; init; }
        [Required(ErrorMessage = "The transaction amount must be provided")]
        [Range(0, int.MaxValue, ErrorMessage = "The transaction amount must be positive")]
        [Currency]
        public decimal? Amount { get; init; }
        [Required(ErrorMessage = "The transaction datetime must be provided")]
        public DateTime? DateTime { get; init; }
        public Guid? FromAccountId { get; init; }
        [AtLeastOneAccount]
        public Guid? ToAccountId { get; init; }
        [NoCategoryIfTransfer]
        public Guid? SubCategoryId { get; init; }
    }
}
