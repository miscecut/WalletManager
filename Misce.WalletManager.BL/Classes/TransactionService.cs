using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.AccountType;
using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.DTO.Enums;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class TransactionService : ITransactionService
    {
        #region Properties

        private WalletManagerContext _walletManagerContext;

        #endregion

        #region CTORs

        public TransactionService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        #endregion

        #region Public Methods

        public TransactionDTOOut? GetTransaction(Guid userId, Guid transactionId)
        {
            var transactionQuery = from transaction in _walletManagerContext.Transactions
                                   where transaction.User.Id == userId
                                   && transaction.Id == transactionId
                                   select transaction;

            if(transactionQuery.Any())
            {
                var transaction = transactionQuery
                    .Include(t => t.FromAccount)
                    .Include(t => t.ToAccount)
                    .Include(t => t.SubCategory)
                    .First();
                Account? fromAccount = null;
                Account? toAccount = null;
                TransactionSubCategory? subCategory = null; 

                if(transaction.SubCategory != null)
                    subCategory = (from tsc in _walletManagerContext.TransactionSubCategories
                                   where tsc.Id == transaction.SubCategory.Id
                                   select tsc)
                                   .Include(tsc => tsc.Category)
                                   .FirstOrDefault();
                if(transaction.FromAccount != null)
                    fromAccount = (from acc in _walletManagerContext.Accounts
                                   where acc.Id == transaction.FromAccount.Id
                                   select acc)
                                   .Include(acc => acc.AccountType)
                                   .FirstOrDefault();
                if(transaction.ToAccount != null)
                    toAccount = (from acc in _walletManagerContext.Accounts
                                 where acc.Id == transaction.ToAccount.Id
                                 select acc)
                                 .Include(acc => acc.AccountType)
                                 .FirstOrDefault();

                return new TransactionDTOOut
                {
                    Id = transaction.Id,
                    Title = transaction.Title,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    FromAccount = fromAccount == null ? null : new AccountReducedDTOOut
                    {
                        Id = fromAccount.Id,
                        Name = fromAccount.Name,
                        IsActive = fromAccount.IsActive,
                        AccountType = new AccountTypeDTOOut
                        {
                            Id = fromAccount.AccountType.Id,
                            Name = fromAccount.AccountType.Name
                        }
                    },
                    ToAccount = toAccount == null ? null : new AccountReducedDTOOut
                    {
                        Id = toAccount.Id,
                        Name = toAccount.Name,
                        IsActive = toAccount.IsActive,
                        AccountType = new AccountTypeDTOOut
                        {
                            Id = toAccount.AccountType.Id,
                            Name = toAccount.AccountType.Name
                        }
                    },
                    DateTime = transaction.DateTime,
                    TransactionSubCategory = subCategory == null ? null : new TransactionSubCategoryDTOOut
                    {
                        Id = subCategory.Id,
                        Name = subCategory.Name,
                        Description = subCategory.Description,
                        TransactionCategory = new TransactionCategoryDTOOut
                        {
                            Id = subCategory.Category.Id,
                            Name = subCategory.Category.Name,
                            Description = subCategory.Category.Description,
                            IsExpenseType = subCategory.Category.IsExpenseCategory
                        }
                    }
                };
            }

            return null;
        }

        public IEnumerable<TransactionDTOOut> GetTransactions(Guid userId, int limit, int page, TransactionType? transactionType = null, string? title = null, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var query = GetFilteredTransactionsQuery(userId, transactionType, title, fromAccountId, toAccountId, categoryId, subCategoryId, fromDateTime, toDateTime);

            //apply page and limit
            query = query.Skip(limit * page).Take(limit);

            return query.Select(t => new TransactionDTOOut
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Amount = t.Amount,
                    DateTime = t.DateTime,
                    FromAccount = t.FromAccount == null ? null : new AccountReducedDTOOut
                    {
                        Id = t.FromAccount.Id,
                        Name = t.FromAccount.Name,
                        IsActive = t.FromAccount.IsActive,
                        AccountType = new AccountTypeDTOOut
                        {
                            Id = t.FromAccount.AccountType.Id,
                            Name = t.FromAccount.AccountType.Name
                        }
                    },
                    ToAccount = t.ToAccount == null ? null : new AccountReducedDTOOut
                    {
                        Id = t.ToAccount.Id,
                        Name = t.ToAccount.Name,
                        IsActive = t.ToAccount.IsActive,
                        AccountType = new AccountTypeDTOOut
                        {
                            Id = t.ToAccount.AccountType.Id,
                            Name = t.ToAccount.AccountType.Name
                        }
                    },
                    TransactionSubCategory = t.SubCategory != null ? new TransactionSubCategoryDTOOut
                    {
                        Id = t.SubCategory.Id,
                        Name = t.SubCategory.Name,
                        Description = t.SubCategory.Description,
                        TransactionCategory = new TransactionCategoryDTOOut
                        {
                            Id = t.SubCategory.Category.Id,
                            Name = t.SubCategory.Category.Name,
                            Description = t.SubCategory.Category.Description,
                            IsExpenseType = t.SubCategory.Category.IsExpenseCategory
                        }
                    } : null
            }).ToList();
        }

        public int GetTransactionsCount(Guid userId, TransactionType? transactionType = null, string ? title = null, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var transactionsQuery = GetFilteredTransactionsQuery(userId, transactionType, title, fromAccountId, toAccountId, categoryId, subCategoryId, fromDateTime, toDateTime);
            return transactionsQuery.Count();
        }

        public TransactionDTOOut CreateTransaction(Guid userId, TransactionCreationDTOIn transaction)
        {
            //transaction creation data validation
            var validationResults = Utils.Utils.ValidateDTO(transaction);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //user check
            var user = GetUser(userId);

            if(user != null)
            {
                //check if the eventual transaction subcategory exists and it's owned by the user
                var subCategoryQuery = from subCategory in _walletManagerContext.TransactionSubCategories
                                       where subCategory.Category.User.Id == userId
                                       && (!transaction.TransactionSubCategoryId.HasValue || transaction.TransactionSubCategoryId.Value == subCategory.Id)
                                       select subCategory;

                if(!transaction.TransactionSubCategoryId.HasValue || subCategoryQuery.Any())
                {
                    //accounts check
                    var userAccountsQuery = from account in _walletManagerContext.Accounts
                                            where account.User.Id == userId
                                            select account;

                    //get the user's account ids
                    var userAccountIds = userAccountsQuery.Select(a => a.Id);
                    //check if the account the transaction is from is owned by the user and exists
                    var fromAccountIdIsValid = !transaction.FromAccountId.HasValue || userAccountIds.Contains(transaction.FromAccountId.Value);
                    //check if the account the transaction is to is owned by the user and exists
                    var toAccountIdIsValid = !transaction.ToAccountId.HasValue || userAccountIds.Contains(transaction.ToAccountId.Value);

                    if (fromAccountIdIsValid && toAccountIdIsValid)
                    {
                        var transactionSubCategory = subCategoryQuery.Include(tsc => tsc.Category).FirstOrDefault();

                        //transaction category type check, if the subcategory is under an expense category, the transaction must be an expense
                        if (transaction.TransactionSubCategoryId.HasValue && transactionSubCategory != null)
                        {
                            var isTransactionCategoryExpenseType = transactionSubCategory.Category.IsExpenseCategory;
                            if (transaction.ToAccountId.HasValue && !transaction.FromAccountId.HasValue && isTransactionCategoryExpenseType)
                                throw new IncorrectDataException(Utils.Utils.SerializeSingleError("TransactionSubCategoryId", "An expense transaction must be created under an expense transaction subcategory"));
                            if (!transaction.ToAccountId.HasValue && transaction.FromAccountId.HasValue && !isTransactionCategoryExpenseType)
                                throw new IncorrectDataException(Utils.Utils.SerializeSingleError("TransactionSubCategoryId", "A profit transaction must be created under a profit transaction subcategory"));
                        }

                        //create the transaction
                        var transactionToCreate = new Transaction
                        {
                            Title = transaction.Title,
                            Description = transaction.Description,
                            Amount = transaction.Amount.GetValueOrDefault(),
                            FromAccount = userAccountsQuery.Where(a => a.Id == transaction.FromAccountId.GetValueOrDefault()).FirstOrDefault(),
                            ToAccount = userAccountsQuery.Where(a => a.Id == transaction.ToAccountId.GetValueOrDefault()).FirstOrDefault(),
                            User = user,
                            DateTime = transaction.DateTime.GetValueOrDefault().ToUniversalTime(),
                            SubCategory = transaction.TransactionSubCategoryId.HasValue ? transactionSubCategory : null
                        };
                        _walletManagerContext.Transactions.Add(transactionToCreate);

                        //commit changes in the db
                        _walletManagerContext.SaveChanges();

                        //return the created transaction's data
                        var createdTransaction = GetTransaction(userId, transactionToCreate.Id);
                        if (createdTransaction != null)
                            return createdTransaction;
                        else
                            throw new Exception();
                    }
                    else
                        throw new IncorrectDataException(Utils.Utils.SerializeSingleError(transaction.FromAccountId != null ? "FromAccountId" : "ToAccountId", "The provided account ID " + (transaction.FromAccountId != null ? transaction.FromAccountId : transaction.ToAccountId) + " was not found"));
                }
                else
                    throw new IncorrectDataException(Utils.Utils.SerializeSingleError("TransactionSubCategoryId", "The provided transaction subcategory ID " + transaction.TransactionSubCategoryId + " was not found"));
            }
            else
                throw new UserNotFoundException();
        }

        public TransactionDTOOut UpdateTransaction(Guid userId, Guid transactionId, TransactionUpdateDTOIn transaction)
        {
            //transaction update data validation
            var validationResults = Utils.Utils.ValidateDTO(transaction);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //check if the transaction exists and it's owned by the user
            var transactionToUpdateQuery = from t in _walletManagerContext.Transactions
                                           where t.User.Id == userId
                                           && t.Id == transactionId
                                           select t;

            if (transactionToUpdateQuery.Any())
            {
                //check if the eventual transaction subcategory exists and it's owned by the user
                var subCategoryQuery = from subCategory in _walletManagerContext.TransactionSubCategories
                                       where subCategory.Category.User.Id == userId
                                       && (!transaction.TransactionSubCategoryId.HasValue || transaction.TransactionSubCategoryId.Value == subCategory.Id)
                                       select subCategory;

                if (!transaction.TransactionSubCategoryId.HasValue || subCategoryQuery.Any())
                {
                    //accounts check
                    var userAccountsQuery = from account in _walletManagerContext.Accounts
                                            where account.User.Id == userId
                                            select account;

                    //get the user's account ids
                    var userAccountIds = userAccountsQuery.Select(a => a.Id);
                    //check if the account the transaction is from is owned by the user and exists
                    var fromAccountIdIsValid = !transaction.FromAccountId.HasValue || userAccountIds.Contains(transaction.FromAccountId.Value);
                    //check if the account the transaction is to is owned by the user and exists
                    var toAccountIdIsValid = !transaction.ToAccountId.HasValue || userAccountIds.Contains(transaction.ToAccountId.Value);

                    if (fromAccountIdIsValid && toAccountIdIsValid)
                    {
                        var transactionSubCategory = subCategoryQuery.FirstOrDefault();

                        //transaction category type check, if the subcategory is under an expense category, the transaction must be an expense
                        if (transactionSubCategory != null)
                        {
                            var isTransactionCategoryExpenseType = transactionSubCategory.Category.IsExpenseCategory;
                            if (transaction.ToAccountId.HasValue && !transaction.FromAccountId.HasValue && isTransactionCategoryExpenseType)
                                throw new IncorrectDataException(Utils.Utils.SerializeSingleError("TransactionSubCategoryId", "An expense transaction must be created under an expense transaction subcategory"));
                            if (!transaction.ToAccountId.HasValue && transaction.FromAccountId.HasValue && !isTransactionCategoryExpenseType)
                                throw new IncorrectDataException(Utils.Utils.SerializeSingleError("TransactionSubCategoryId", "A profit transaction must be created under a profit transaction subcategory"));
                        }

                        //update the transaction
                        var transactionToUpdate = transactionToUpdateQuery.First();
                        transactionToUpdate.SubCategory = transaction.TransactionSubCategoryId.HasValue ? subCategoryQuery.First() : null;
                        transactionToUpdate.FromAccount = userAccountsQuery.Where(a => a.Id == transaction.FromAccountId.GetValueOrDefault()).FirstOrDefault();
                        transactionToUpdate.ToAccount = userAccountsQuery.Where(a => a.Id == transaction.ToAccountId.GetValueOrDefault()).FirstOrDefault();
                        transactionToUpdate.Title = transaction.Title;
                        transactionToUpdate.Description = transaction.Description;
                        transactionToUpdate.Amount = transaction.Amount.GetValueOrDefault();
                        transactionToUpdate.DateTime = transaction.DateTime.GetValueOrDefault().ToUniversalTime();

                        //commit changes in the db
                        _walletManagerContext.SaveChanges();

                        //return the updated transaction's data
                        var updatedTransaction = GetTransaction(userId, transactionId);
                        if (updatedTransaction != null)
                            return updatedTransaction;
                        else
                            throw new Exception();
                    }
                    else
                        throw new IncorrectDataException(Utils.Utils.SerializeSingleError(transaction.FromAccountId != null ? "FromAccountId" : "ToAccountId", "The provided account ID " + (transaction.FromAccountId != null ? transaction.FromAccountId : transaction.ToAccountId) + " was not found"));
                }
                else
                    throw new IncorrectDataException(Utils.Utils.SerializeSingleError("TransactionSubCategoryId", "The provided transaction subcategory ID " + transaction.TransactionSubCategoryId + " was not found"));
            }
            else
                throw new ElementNotFoundException();
        }

        public void DeleteTransaction(Guid userId, Guid transactionId)
        {
            //check if the transaction exists and it's owned by the user
            var transactionToDelete = from transaction in _walletManagerContext.Transactions
                                      where transaction.User.Id == userId
                                      && transaction.Id == transactionId
                                      select transaction;

            if (transactionToDelete.Any())
            {
                //delete the transaction
                _walletManagerContext.Transactions.Remove(transactionToDelete.First());

                //commit changes in the db
                _walletManagerContext.SaveChanges();
            }
            else
                throw new ElementNotFoundException();
        }

        #endregion

        #region Private Methods

        private User? GetUser(Guid id)
        {
            var userQuery = from u in _walletManagerContext.Users
                            where id == u.Id
                            select u;

            return userQuery.FirstOrDefault();
        }

        private IQueryable<Transaction> GetFilteredTransactionsQuery(Guid userId, TransactionType? transactionType = null, string ? title = null, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var transactionsQuery = from transaction in _walletManagerContext.Transactions
                                    where transaction.User.Id == userId
                                    select transaction;
            //apply filters
            if(transactionType != null)
            {
                if(transactionType == TransactionType.EXPENSE)
                    transactionsQuery = transactionsQuery.Where(t => t.FromAccount != null && t.ToAccount == null);
                else if (transactionType == TransactionType.PROFIT)
                    transactionsQuery = transactionsQuery.Where(t => t.FromAccount == null && t.ToAccount != null);
                else //TRANSFER
                    transactionsQuery = transactionsQuery.Where(t => t.FromAccount != null && t.ToAccount != null);
            }
            if (title != null)
                transactionsQuery = transactionsQuery.Where(t => t.Title != null && t.Title.ToUpper().Contains(title.ToUpper()));
            if (fromAccountId != null)
                transactionsQuery = transactionsQuery.Where(t => t.FromAccount != null && t.FromAccount.Id == fromAccountId);
            if (toAccountId != null)
                transactionsQuery = transactionsQuery.Where(t => t.ToAccount != null && t.ToAccount.Id == toAccountId);
            if (categoryId != null)
                transactionsQuery = transactionsQuery.Where(t => t.SubCategory != null && t.SubCategory.Category.Id == categoryId);
            if (subCategoryId != null)
                transactionsQuery = transactionsQuery.Where(t => t.SubCategory != null && t.SubCategory.Id == subCategoryId);
            if (fromDateTime != null)
                transactionsQuery = transactionsQuery.Where(t => t.DateTime >= fromDateTime);
            if (toDateTime != null)
                transactionsQuery = transactionsQuery.Where(t => t.DateTime <= toDateTime);

            return transactionsQuery.OrderByDescending(t => t.DateTime); //order by datetime desc
        }

        #endregion
    }
}
