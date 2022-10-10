using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.AccountType;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.BL.Classes
{
    public class AccountTypeService : IAccountTypeService
    {
        private WalletManagerContext _walletManagerContext;

        public AccountTypeService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        public IEnumerable<AccountTypeDTOOut> GetAccountTypes()
        {
            var query = from accountType in _walletManagerContext.AccountTypes
                        select new AccountTypeDTOOut
                        {
                            Id = accountType.Id,
                            Name = accountType.Name
                        };

            return query.ToList();
        }
    }
}
