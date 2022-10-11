namespace Misce.WalletManager.DTO.DTO.TransactionCategory
{
    public record TransactionCategoryDTOOut
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public bool IsExpenseType { get; init; }
    }
}
