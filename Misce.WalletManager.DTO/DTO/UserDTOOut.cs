namespace Misce.WalletManager.DTO.DTO
{
    public record UserDTOOut
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = null!;
    }
}
