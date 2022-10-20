using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.AccountType;
using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;
using System.Text.Json;

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
                            FromAccount = transaction.FromAccount == null ? null : new AccountReducedDTOOut
                            {
                                Id = transaction.FromAccount.Id,
                                Name = transaction.FromAccount.Name,
                                IsActive = transaction.FromAccount.IsActive,
                                AccountType = new AccountTypeDTOOut
                                {
                                    Id = transaction.FromAccount.AccountType.Id,
                                    Name = transaction.FromAccount.AccountType.Name
                                }
                            },
                            ToAccount = transaction.ToAccount == null ? null : new AccountReducedDTOOut
                            {
                                Id = transaction.ToAccount.Id,
                                Name = transaction.ToAccount.Name,
                                IsActive = transaction.ToAccount.IsActive,
                                AccountType = new AccountTypeDTOOut
                                {
                                    Id = transaction.ToAccount.AccountType.Id,
                                    Name = transaction.ToAccount.AccountType.Name
                                }
                            },
                            DateTime = transaction.DateTime,
                            TransactionSubCategory = transaction.SubCategory != null ? new TransactionSubCategoryDTOOut
                            {
                                Id = subCategorySubquery.Id,
                                Name = subCategorySubquery.Name,
                                Description = subCategorySubquery.Description,
                                TransactionCategory = new TransactionCategoryDTOOut
                                {
                                    Id = subCategorySubquery.Category.Id,
                                    Name = subCategorySubquery.Category.Name,
                                    Description = subCategorySubquery.Category.Description,
                                    IsExpenseType = subCategorySubquery.Category.IsExpenseCategory
                                }
                            } : null
                        };

            return query.FirstOrDefault();
        }

        public IEnumerable<TransactionDTOOut> GetTransactions(Guid userId, int limit, int page, string? title = null,Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var query = GetFilteredTransactionsQuery(userId, title, fromAccountId, toAccountId, categoryId, subCategoryId, fromDateTime, toDateTime);

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

        public int GetTransactionsCount(Guid userId, string? title = null, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var transactionsQuery = GetFilteredTransactionsQuery(userId, title, fromAccountId, toAccountId, categoryId, subCategoryId, fromDateTime, toDateTime);
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
                                throw new IncorrectDataException("An expense transaction must be created under an expense transaction subcategory");
                            if (!transaction.ToAccountId.HasValue && transaction.FromAccountId.HasValue && !isTransactionCategoryExpenseType)
                                throw new IncorrectDataException("A profit transaction must be created under a profit transaction subcategory");
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
                            SubCategory = transactionSubCategory
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
                        throw new IncorrectDataException();
                }
                else
                    throw new IncorrectDataException(JsonSerializer.Serialize(new ErrorContainer("TransactionSubCategoryId", "The provided transaction ID " + transaction.TransactionSubCategoryId + " was not found")));
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
                                       && (!transaction.SubCategoryId.HasValue || transaction.SubCategoryId.Value == subCategory.Id)
                                       select subCategory;

                if (!transaction.SubCategoryId.HasValue || subCategoryQuery.Any())
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
                                throw new IncorrectDataException("An expense transaction must be created under an expense transaction subcategory");
                            if (!transaction.ToAccountId.HasValue && transaction.FromAccountId.HasValue && !isTransactionCategoryExpenseType)
                                throw new IncorrectDataException("A profit transaction must be created under a profit transaction subcategory");
                        }

                        //update the transaction
                        var transactionToUpdate = transactionToUpdateQuery.First();
                        transactionToUpdate.SubCategory = transaction.SubCategoryId.HasValue ? subCategoryQuery.First() : null;
                        transactionToUpdate.FromAccount = userAccountsQuery.Where(a => a.Id == transaction.FromAccountId.GetValueOrDefault()).FirstOrDefault();
                        transactionToUpdate.ToAccount = userAccountsQuery.Where(a => a.Id == transaction.ToAccountId.GetValueOrDefault()).FirstOrDefault();
                        transactionToUpdate.Title = transaction.Title;
                        transactionToUpdate.Description = transaction.Description;
                        transactionToUpdate.Amount = transaction.Amount.GetValueOrDefault();
                        transactionToUpdate.DateTime = transaction.DateTime.GetValueOrDefault().ToUniversalTime();
                        transactionToUpdate.LastModifiedDateTime = DateTime.UtcNow;

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
                        throw new IncorrectDataException("The account ID was not found");
                }
                else
                    throw new IncorrectDataException("The transaction subcategory ID " + transaction.SubCategoryId + " was not found");
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

        private IQueryable<Transaction> GetFilteredTransactionsQuery(Guid userId, string? title = null, Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var transactionsQuery = from transaction in _walletManagerContext.Transactions
                                    where transaction.User.Id == userId
                                    select transaction;
            //apply filters
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

            return transactionsQuery;
        }

        #endregion
    }
}
