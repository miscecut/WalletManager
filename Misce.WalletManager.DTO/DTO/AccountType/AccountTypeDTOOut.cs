namespace Misce.WalletManager.DTO.DTO.AccountType
{
    public record AccountTypeDTOOut
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
    }
}
