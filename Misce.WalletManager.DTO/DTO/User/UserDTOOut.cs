namespace Misce.WalletManager.DTO.DTO.User
{
    public record UserDTOOut
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = null!;
    }
}
