namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountUpdateDTOIn
    {
        public Guid AccountTypeId { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public decimal InitialAmount { get; init; }
        public bool IsActive { get; init; }
    }
}
