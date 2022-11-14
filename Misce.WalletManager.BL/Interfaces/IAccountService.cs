using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.Enums;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IAccountService
    {
        //CRUD
        public AccountDTOOut? GetAccount(Guid userId, Guid Id);
        public IEnumerable<AccountDTOOut> GetAccounts(Guid userId, Guid? accountTypeId = null, bool? active = null);
        public AccountDTOOut CreateAccount(Guid userId, AccountCreationDTOIn account);
        public AccountDTOOut UpdateAccount(Guid userId, Guid accountId, AccountUpdateDTOIn account);
        public void DeleteAccount(Guid userId, Guid accountId);
        //SPECIAL
        public IEnumerable<AccountWithAmountHistoryDTOOut> GetAccountsWithAmountHistory(Guid userId, bool? active = null, GroupByPeriod groupByPeriod = GroupByPeriod.MONTH);
    }
}
