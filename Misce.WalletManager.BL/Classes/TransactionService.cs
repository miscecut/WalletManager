﻿using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
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

            query = query.Skip(limit * page).Take(limit);

            return query.Select(t => new TransactionDTOOut
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Amount = t.Amount,
                    FromAccountName = t.FromAccount == null ? null : t.FromAccount.Name,
                    ToAccountName = t.ToAccount == null ? null : t.ToAccount.Name,
                    DateTime = t.DateTime,
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
            //user check
            var user = GetUser(userId);

            if(user != null)
            {
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

                        return new TransactionDTOOut
                        {
                            Id = transactionToCreate.Id,
                            Title = transactionToCreate.Title,
                            Description = transactionToCreate.Description,
                            Amount = transactionToCreate.Amount,
                            FromAccountName = transactionToCreate.FromAccount?.Name ?? null,
                            ToAccountName = transactionToCreate.ToAccount?.Name ?? null,
                            DateTime = transactionToCreate.DateTime,
                            TransactionSubCategory = transactionToCreate.SubCategory != null ? new TransactionSubCategoryDTOOut
                            {
                                Id = transactionToCreate.SubCategory.Id,
                                Name = transactionToCreate.SubCategory.Name,
                                Description = transactionToCreate.SubCategory.Description,
                                TransactionCategory = new TransactionCategoryDTOOut
                                {
                                    Id = transactionToCreate.SubCategory.Category.Id,
                                    Name = transactionToCreate.SubCategory.Category.Name,
                                    Description = transactionToCreate.SubCategory.Category.Description
                                }
                            } : null
                        };
                    }
                    //account not valid or not found
                    throw new InvalidDataException("The provided account id is not valid");
                }
                throw new InvalidDataException("The provided sub category id is not valid");
            }
            //user not found
            throw new InvalidDataException("The provided user id is not valid");
        }

        public void DeleteTransaction(Guid userId, Guid transactionId)
        {
            var user = GetUser(userId);

            if(user != null)
            {
                var transactionToDelete = from transaction in _walletManagerContext.Transactions
                                          where transaction.User.Id == user.Id
                                          && transaction.Id == transactionId
                                          select transaction;

                if (transactionToDelete.Any())
                {
                    _walletManagerContext.Transactions.Remove(transactionToDelete.First());
                    _walletManagerContext.SaveChanges();
                }
                else
                    throw new InvalidDataException("The provided transaction id is not valid");
            }
            //user not found
            else
                throw new InvalidDataException("The provided user id is not valid");
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
