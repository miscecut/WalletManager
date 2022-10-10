using Misce.WalletManager.DTO.DTO.AccountType;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IAccountTypeService
    {
        public IEnumerable<AccountTypeDTOOut> GetAccountTypes();
    }
}
