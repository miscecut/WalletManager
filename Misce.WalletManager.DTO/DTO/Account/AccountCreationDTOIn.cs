namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountCreationDTOIn
    {
        public Guid AccountTypeId { get; init; }
        public int InitialAmount { get; init; } = 0;
        public string? Description { get; init; }
        public string Name { get; init; } = null!;
        public bool IncludeInTotal { get; init; } = true;
    }
}
