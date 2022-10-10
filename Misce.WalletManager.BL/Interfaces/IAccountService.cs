using Misce.WalletManager.DTO.DTO.Account;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IAccountService
    {
        public AccountDTOOut? GetAccount(Guid id, Guid userId);
        public IEnumerable<AccountDTOOut> GetAccounts(Guid userId, Guid? accountTypeId = null, bool? active = null);
        public AccountDTOOut CreateAccount(Guid userId, AccountCreationDTOIn account);
        public AccountDTOOut UpdateAccount(Guid userId, Guid accountId, AccountUpdateDTOIn account);
        public void DeleteAccount(Guid userId, Guid accountId);
    }
}
