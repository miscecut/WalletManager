namespace Misce.WalletManager.DTO.DTO.User
{
    public record LoggedUserDTOOut
    {
        public string Username { get; init; } = null!;
        public string Token { get; init; } = null!;
    }
}
