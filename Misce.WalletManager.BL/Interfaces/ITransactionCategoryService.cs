using Misce.WalletManager.DTO.DTO.TransactionCategory;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ITransactionCategoryService
    {
        public IEnumerable<TransactionCategoryDTOOut> GetTransactionCategories(Guid userId);
        public TransactionCategoryDTOOut CreateTransactionCategory(Guid userId, TransactionCategoryCreationDTOIn transactionCategory);
        public TransactionCategoryDTOOut? UpdateTransactionCategory(Guid userId, Guid transactionCategoryId, TransactionCategoryUpdateDTOIn transactionCategory);
    }
}
