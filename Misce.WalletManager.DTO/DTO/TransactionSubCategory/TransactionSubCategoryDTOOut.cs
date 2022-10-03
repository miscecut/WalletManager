using Misce.WalletManager.DTO.DTO.TransactionCategory;

namespace Misce.WalletManager.DTO.DTO.TransactionSubCategory
{
    public record TransactionSubCategoryDTOOut
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public TransactionCategoryDTOOut Category { get; init; } = null!;
    }
}
