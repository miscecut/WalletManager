using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.TransactionCategory
{
    public record TransactionCategoryUpdateDTOIn
    {
        [Required(ErrorMessage = "The transaction category id must provided")]
        public Guid Id { get; init; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "The transaction category name must be provided")]
        [MaxLength(50, ErrorMessage = "The transaction category name is too long")]
        public string Name { get; init; } = null!;
        [MaxLength(500, ErrorMessage = "The transaction category description is too long")]
        public string? Description { get; init; }
        [Required(ErrorMessage = "The transaction category type must be provided")]
        public bool IsExpenseType { get; init; }
    }
}
