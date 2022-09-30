namespace Misce.WalletManager.DTO.DTO
{
    public record UserLoginDTOIn
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
