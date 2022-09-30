using Misce.WalletManager.DTO.DTO.TransactionCategory;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ITransactionCategoryService
    {
        public IEnumerable<TransactionCategoryDTOOut> GetTransactionCategories(Guid userId);
        public Guid CreateTransactionCategory(Guid userId, TransactionCategoryCreationDTOIn transactionCategory);
        public Guid UpdateTransactionCategory(Guid userId, Guid transactionCategoryId, TransactionCategoryUpdateDTOIn transactionCategory);
    }
}
