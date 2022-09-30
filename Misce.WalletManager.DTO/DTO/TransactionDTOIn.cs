namespace Misce.WalletManager.DTO.DTO
{
    public record TransactionDTOIn
    {
        public string? Title { get; init; }
        public string? Description { get; init; }
        public decimal Amount { get; init; }
        public Guid? FromAccountId { get; init; }
        public Guid? ToAccountId { get; init; }
        public DateTime DateTime { get; init; }
        public Guid? SubCategoryId { get; init; }
    }
}
