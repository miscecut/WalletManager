using Misce.WalletManager.DTO.DTO;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ITransactionService
    {
        public TransactionDTOOut? GetTransaction(Guid userId, Guid transactionId);
        public IEnumerable<TransactionDTOOut> GetTransactions(Guid userId, int limit, int page, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null);
    }
}
