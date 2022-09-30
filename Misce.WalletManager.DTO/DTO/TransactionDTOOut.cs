namespace Misce.WalletManager.DTO.DTO
{
    public record TransactionDTOOut
    {
        public Guid Id { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public decimal Amount { get; init; }
        public string? FromAccountName { get; init; }
        public string? ToAccountName { get; init; }
        public DateTime DateTime { get; init; }
        public string? SubCategory { get; init; }
        public string? Category { get; init; }
    }
}
