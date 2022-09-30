using Misce.WalletManager.DTO.DTO.Account;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IAccountService
    {
        public Guid CreateAccount(Guid userId, AccountCreationDTOIn account);
        public AccountDTOOut? GetAccount(Guid id, Guid userId);
        public IEnumerable<AccountDTOOut> GetAccounts(Guid userId, bool? active = null);
        public Guid UpdateAccount(Guid userId, Guid accountId, AccountUpdateDTOIn account);
    }
}
