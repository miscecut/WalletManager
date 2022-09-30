using Misce.WalletManager.DTO.DTO;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface IAccountTypeService
    {
        public IEnumerable<AccountTypeDTOOut> GetAccountTypes();
    }
}
