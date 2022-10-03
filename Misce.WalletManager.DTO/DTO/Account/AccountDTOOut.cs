namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountDTOOut
    {
        public Guid Id { get; init; }
        public AccountTypeDTOOut AccountType { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public decimal Amount { get; init; }
        public bool IsIncludedInTotal { get; init; }
    }
}
