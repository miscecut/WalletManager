using Misce.WalletManager.DTO.DTO.TransactionSubCategory;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ISubCategoryService
    {
        public IEnumerable<TransactionSubCategoryDTOOut> GetTransactionSubCategories(Guid userId);
        public TransactionSubCategoryDTOOut CreateTransactionSubCategory(Guid userId, TransactionSubCategoryCreationDTOIn subCategory);
        public TransactionSubCategoryDTOOut UpdateTransactionSubCategory(Guid userId, Guid transactionSubCategory, TransactionSubCategoryUpdateDTOIn subCategory);
    }
}
