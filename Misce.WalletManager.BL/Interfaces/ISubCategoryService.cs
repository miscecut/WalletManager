using Misce.WalletManager.DTO.DTO.TransactionSubCategory;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ISubCategoryService
    {
        public IEnumerable<TransactionSubCategoryDTOOut> GetSubCategories(Guid userId);
        public Guid CreateSubCategory(Guid userId, TransactionSubCategoryCreationDTOIn subCategory);
    }
}
