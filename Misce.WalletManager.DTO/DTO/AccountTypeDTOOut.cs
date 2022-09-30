namespace Misce.WalletManager.DTO.DTO
{
    public record AccountTypeDTOOut
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
    }
}
