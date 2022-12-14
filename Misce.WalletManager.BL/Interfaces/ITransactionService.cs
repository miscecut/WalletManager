using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.Enums;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ITransactionService
    {
        public TransactionDTOOut? GetTransaction(Guid userId, Guid transactionId);
        public IEnumerable<TransactionDTOOut> GetTransactions(Guid userId, int limit, int page, TransactionType? transactionType = null, string? title = null,Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null);
        public int GetTransactionsCount(Guid userId, TransactionType? transactionType = null, string ? title = null, Guid ? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null);
        public TransactionDTOOut CreateTransaction(Guid userId, TransactionCreationDTOIn transaction);
        public TransactionDTOOut UpdateTransaction(Guid userId, Guid transactionId, TransactionUpdateDTOIn transaction);
        public void DeleteTransaction(Guid userId, Guid transactionId);
    }
}
