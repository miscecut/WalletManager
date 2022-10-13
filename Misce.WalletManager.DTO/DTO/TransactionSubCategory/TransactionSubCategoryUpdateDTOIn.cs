using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.TransactionSubCategory
{
    public record TransactionSubCategoryUpdateDTOIn
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The transaction subcategory name must be provided")]
        [MaxLength(50, ErrorMessage = "The transaction subcategory name is too long")]
        public string Name { get; init; } = null!;
        [MaxLength(500, ErrorMessage = "The transaction subcategory description is too long")]
        public string? Description { get; init; }
        [Required(ErrorMessage = "The transaction category id must be provided")]
        public Guid TransactionCategoryId { get; init; }
    }
}
