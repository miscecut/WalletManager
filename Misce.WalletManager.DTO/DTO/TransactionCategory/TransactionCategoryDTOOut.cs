namespace Misce.WalletManager.DTO.DTO.TransactionCategory
{
    public record TransactionCategoryDTOOut
    {
        public Guid Id;
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }
}
