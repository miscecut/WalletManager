using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class TransactionService : ITransactionService
    {
        private WalletManagerContext _walletManagerContext;

        public TransactionService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        public TransactionDTOOut? GetTransaction(Guid userId, Guid transactionId)
        {
            var query = from transaction in _walletManagerContext.Transactions
                        from account in _walletManagerContext.Accounts
                        join subCategory in _walletManagerContext.TransactionSubCategories
                            on transaction.SubCategory equals subCategory into subCategories
                        from subCategorySubquery in subCategories.DefaultIfEmpty()
                        join category in _walletManagerContext.TransactionCategories
                            on subCategorySubquery.Category equals category
                        where transaction.User.Id == userId
                        && transaction.Id == transactionId
                        && (account == transaction.FromAccount || account == transaction.ToAccount)
                        select new TransactionDTOOut
                        {
                            Id = transactionId,
                            Title = transaction.Title,
                            Description = transaction.Description,
                            Amount = transaction.Amount,
                            FromAccountName = transaction.FromAccount == null ? null : transaction.FromAccount.Name,
                            ToAccountName = transaction.FromAccount == null ? null : transaction.ToAccount.Name,
                            SubCategory = subCategorySubquery.Name,
                            Category = category.Name,
                            DateTime = transaction.DateTime
                        };

            return query.FirstOrDefault();
        }

        public IEnumerable<TransactionDTOOut> GetTransactions(Guid userId, int limit, int page, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var query = from transaction in _walletManagerContext.Transactions
                        where transaction.User.Id == userId
                        select transaction;

            if (fromAccountId != null)
                query = query.Where(t => t.FromAccount != null && t.FromAccount.Id == fromAccountId);
            if (toAccountId != null)
                query = query.Where(t => t.ToAccount != null && t.ToAccount.Id == toAccountId);
            if(categoryId != null)
                query = query.Where(t => t.SubCategory != null && t.SubCategory.Category.Id == categoryId);
            if (subCategoryId != null)
                query = query.Where(t => t.SubCategory != null && t.SubCategory.Id == subCategoryId);
            if (fromDateTime != null)
                query = query.Where(t => t.DateTime >= fromDateTime);
            if (toDateTime != null)
                query = query.Where(t => t.DateTime <= toDateTime);

            query = query.Skip(limit * page).Take(limit);

            return query.Select(t => new TransactionDTOOut
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Amount = t.Amount,
                    FromAccountName = t.FromAccount == null ? null : t.FromAccount.Name,
                    ToAccountName = t.ToAccount == null ? null : t.ToAccount.Name,
                    SubCategory = t.SubCategory == null ? null : t.SubCategory.Name,
                    Category = t.SubCategory == null ? null : t.SubCategory.Category.Name,
                    DateTime = t.DateTime
                }).ToList();
        }
    
        public Guid CreateTransaction(Guid userId, TransactionDTOIn transaction)
        {
            //user check
            var user = GetUser(userId);
            if(user != null)
            {
                var subCategoryQuery = from subCategory in _walletManagerContext.TransactionSubCategories
                                       where subCategory.Category.User.Id == userId
                                       && (!transaction.SubCategoryId.HasValue || transaction.SubCategoryId.Value == subCategory.Id)
                                       select subCategory;

                if(subCategoryQuery.Any() || !transaction.SubCategoryId.HasValue)
                {
                    //accounts check
                    var userAccounts = from account in _walletManagerContext.Accounts
                                       where (!transaction.FromAccountId.HasValue || transaction.FromAccountId == account.Id)
                                       && (!transaction.ToAccountId.HasValue || transaction.ToAccountId == account.Id)
                                       && account.User.Id == userId
                                       select account;

                    if(userAccounts.Any())
                    {
                        var transactionToCreate = new Transaction
                        {
                            Title = transaction.Title,
                            Description = transaction.Description,
                            Amount = transaction.Amount,
                            FromAccount = userAccounts.Where(a => a.Id == transaction.FromAccountId.GetValueOrDefault()).FirstOrDefault(),
                            ToAccount = userAccounts.Where(a => a.Id == transaction.ToAccountId.GetValueOrDefault()).FirstOrDefault(),
                            User = user,
                            DateTime = transaction.DateTime,
                            SubCategory = subCategoryQuery.FirstOrDefault()
                        };

                        _walletManagerContext.Transactions.Add(transactionToCreate);
                        _walletManagerContext.SaveChanges();

                        return transactionToCreate.Id;
                    }
                    //account not valid or not found
                    throw new InvalidDataException("The provided account id is not valid!");
                }
                throw new InvalidDataException("The provided sub category id is not valid!");
            }
            //user not found
            throw new InvalidDataException("The provided user id is not valid!");
        }
        private User? GetUser(Guid id)
        {
            var userQuery = from u in _walletManagerContext.Users
                            where id == u.Id
                            select u;

            return userQuery.FirstOrDefault();
        }
    }
}
