namespace Misce.WalletManager.DTO.DTO.TransactionSubCategory
{
    public record TransactionSubCategoryUpdateDTOIn
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public Guid TransactionCategoryId { get; init; }
    }
}
