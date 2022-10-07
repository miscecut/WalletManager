using Misce.WalletManager.DTO.DTO.TransactionSubCategory;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ITransactionSubCategoryService
    {
        public TransactionSubCategoryDTOOut? GetTransactionSubCategory(Guid userId, Guid transactionSubCategoryId);
        public IEnumerable<TransactionSubCategoryDTOOut> GetTransactionSubCategories(Guid userId, Guid? transactionCategoryId = null, string? name = null);
        public TransactionSubCategoryDTOOut CreateTransactionSubCategory(Guid userId, TransactionSubCategoryCreationDTOIn subCategory);
        public TransactionSubCategoryDTOOut? UpdateTransactionSubCategory(Guid userId, Guid transactionSubCategory, TransactionSubCategoryUpdateDTOIn subCategory);
        public void DeleteTransactionSubCategory(Guid userId, Guid transactionSubCategoryId);
    }
}
