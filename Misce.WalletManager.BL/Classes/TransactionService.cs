using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.AccountType;
using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
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
                                    Description = subCategorySubquery.Category.Description
                                }
                            } : null
                        };

            return query.FirstOrDefault();
        }

        public IEnumerable<TransactionDTOOut> GetTransactions(Guid userId, int limit, int page, string? title = null,Guid? fromAccountId = null, Guid? toAccountId = null, Guid? categoryId = null, Guid? subCategoryId = null, DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            var query = from transaction in _walletManagerContext.Transactions
                        where transaction.User.Id == userId
                        select transaction;

            //apply filters
            if (title != null)
                query = query.Where(t => t.Title != null && t.Title.ToUpper().Contains(title.ToUpper()));
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
                            Description = t.SubCategory.Category.Description
                        }
                    } : null
            }).ToList();
        }
    
        public TransactionDTOOut CreateTransaction(Guid userId, TransactionCreationDTOIn transaction)
        {
            //transaction creation data validation
            var validationResults = Utils.Utils.ValidateDTO(transaction);
            if (!string.IsNullOrEmpty(validationResults))
                throw new IncorrectDataException(validationResults);

            //user check
            var user = GetUser(userId);

            if(user != null)
            {
                //check if the eventual transaction subcategory exists and it's owned by the user
                var subCategoryQuery = from subCategory in _walletManagerContext.TransactionSubCategories
                                       where subCategory.Category.User.Id == userId
                                       && (!transaction.SubCategoryId.HasValue || transaction.SubCategoryId.Value == subCategory.Id)
                                       select subCategory;

                if(!transaction.SubCategoryId.HasValue || subCategoryQuery.Any())
                {
                    //accounts check
                    var userAccounts = from account in _walletManagerContext.Accounts
                                       where (!transaction.FromAccountId.HasValue || transaction.FromAccountId == account.Id)
                                       && (!transaction.ToAccountId.HasValue || transaction.ToAccountId == account.Id)
                                       && account.User.Id == userId
                                       select account;

                    if((transaction.FromAccountId != null || transaction.ToAccountId != null) && userAccounts.Any())
                    {
                        //create the transaction
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

                        //commit changes in the db
                        _walletManagerContext.SaveChanges();

                        //return the created transaction's data
                        var createdTransaction = GetTransaction(userId, transactionToCreate.Id);
                        if (createdTransaction != null)
                            return createdTransaction;
                        else
                            throw new Exception("An internal server error occurred");
                    }
                    else
                        throw new IncorrectDataException("The account ID was not found");
                }
                else
                    throw new IncorrectDataException("The transaction subcategory ID " + transaction.SubCategoryId + " was not found");
            }
            else
                throw new UserNotFoundException();
        }

        public TransactionDTOOut UpdateTransaction(Guid userId, Guid transactionId, TransactionUpdateDTOIn transaction)
        {
            //transaction update data validation
            var validationResults = Utils.Utils.ValidateDTO(transaction);
            if (!string.IsNullOrEmpty(validationResults))
                throw new IncorrectDataException(validationResults);

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
                    var userAccounts = from account in _walletManagerContext.Accounts
                                       where (!transaction.FromAccountId.HasValue || transaction.FromAccountId == account.Id)
                                       && (!transaction.ToAccountId.HasValue || transaction.ToAccountId == account.Id)
                                       && account.User.Id == userId
                                       select account;

                    if ((transaction.FromAccountId != null || transaction.ToAccountId != null) && userAccounts.Any())
                    {
                        //update the transaction
                        var transactionToUpdate = transactionToUpdateQuery.First();
                        transactionToUpdate.SubCategory = transaction.SubCategoryId.HasValue ? subCategoryQuery.First() : null;
                        transactionToUpdate.FromAccount = userAccounts.Where(a => a.Id == transaction.FromAccountId.GetValueOrDefault()).FirstOrDefault();
                        transactionToUpdate.ToAccount = userAccounts.Where(a => a.Id == transaction.ToAccountId.GetValueOrDefault()).FirstOrDefault();
                        transactionToUpdate.Title = transaction.Title;
                        transactionToUpdate.Description = transaction.Description;
                        transactionToUpdate.Amount = transaction.Amount;
                        transactionToUpdate.DateTime = transaction.DateTime;
                        transactionToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                        //commit changes in the db
                        _walletManagerContext.SaveChanges();

                        //return the updated transaction's data
                        var updatedTransaction = GetTransaction(userId, transactionId);
                        if (updatedTransaction != null)
                            return updatedTransaction;
                        else
                            throw new Exception("An internal server error occurred");
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

        #endregion
    }
}
