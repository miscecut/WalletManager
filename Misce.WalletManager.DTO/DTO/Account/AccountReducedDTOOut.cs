using Misce.WalletManager.DTO.DTO.AccountType;

namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountReducedDTOOut
    {
        public Guid Id { get; init; }
        public AccountTypeDTOOut AccountType { get; init; } = null!;
        public string Name { get; init; } = null!;
        public bool IsActive { get; init; }
    }
}
