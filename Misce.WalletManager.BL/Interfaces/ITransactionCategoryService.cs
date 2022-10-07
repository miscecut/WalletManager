using Misce.WalletManager.DTO.DTO.TransactionCategory;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ITransactionCategoryService
    {
        public TransactionCategoryDTOOut? GetTransactionCategory(Guid userId, Guid transactioNCategoryId);
        public IEnumerable<TransactionCategoryDTOOut> GetTransactionCategories(Guid userId, string? name = null);
        public TransactionCategoryDTOOut CreateTransactionCategory(Guid userId, TransactionCategoryCreationDTOIn transactionCategory);
        public TransactionCategoryDTOOut UpdateTransactionCategory(Guid userId, Guid transactionCategoryId, TransactionCategoryUpdateDTOIn transactionCategory);
        public void DeleteTransactionCategory(Guid userId, Guid transactionCategoryId);
    }
}
