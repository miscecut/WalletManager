using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.AccountType;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class AccountService : IAccountService
    {
        #region Properties

        private WalletManagerContext _walletManagerContext;

        #endregion

        #region CTORs

        public AccountService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        #endregion

        #region Public Methods

        public AccountDTOOut? GetAccount(Guid userId, Guid id)
        {
            var query = from a in _walletManagerContext.Accounts
                        where a.Id == id
                        && a.User.Id == userId
                        select a;

            if (query.Any())
            {
                var account = query.Include(a => a.AccountType).First();

                //actual gains = summation(amount of the transaction with the account as account to)
                var toAccountAmountQuery = from t in _walletManagerContext.Transactions
                                           where t.ToAccount != null
                                           && t.ToAccount.Id == id
                                           select t;

                var moneyIn = toAccountAmountQuery
                    .Select(t => t.Amount)
                    .Sum();

                //actual costs = summation(amount of the transaction with the account as account from)
                var FromAccountAmountQuery = from t in _walletManagerContext.Transactions
                                             where t.FromAccount != null
                                             && t.FromAccount.Id == id
                                             select t;

                var moneyOut = FromAccountAmountQuery
                    .Select(t => t.Amount)
                    .Sum();

                var actualAccountAmount = account.InitialAmount + moneyIn - moneyOut;

                return new AccountDTOOut
                {
                    Id = account.Id,
                    Name = account.Name,
                    InitialAmount = account.InitialAmount,
                    ActualAmount = actualAccountAmount,
                    IsActive = account.IsActive,
                    Description = account.Description,
                    AccountType = new AccountTypeDTOOut
                    {
                        Id = account.AccountType.Id,
                        Name = account.AccountType.Name
                    }
                };
            }

            return null;
        }

        public IEnumerable<AccountDTOOut> GetAccounts(Guid userId, Guid? accountTypeId = null, bool? active = null)
        {
            var query = from a in _walletManagerContext.Accounts.Include(a => a.AccountType)
                        join at in _walletManagerContext.AccountTypes
                            on a.AccountType.Id equals at.Id
                        where a.User.Id == userId
                        select a;

            if (active != null)
                query = query.Where(a => a.IsActive == active);
            if(accountTypeId != null)
                query = query.Where(a => a.AccountType.Id == accountTypeId);

            if (query.Any())
            {
                var accounts = query.ToList();

                var accountAmountsQuery = from t in _walletManagerContext.Transactions
                                          where t.User.Id == userId
                                          select t;

                var transactions = accountAmountsQuery.ToList();

                //find out how much money got in and out from every account
                var moneyInMap = new Dictionary<Guid, decimal>();
                var moneyOutMap = new Dictionary<Guid, decimal>();
                //init maps
                foreach (var account in accounts)
                {
                    moneyInMap[account.Id] = 0;
                    moneyOutMap[account.Id] = 0;
                }
                //fill maps with the transactions for every account
                foreach (var transaction in transactions)
                {
                    if (transaction.FromAccount != null)
                        moneyOutMap[transaction.FromAccount.Id] = moneyOutMap[transaction.FromAccount.Id] + transaction.Amount;
                    if (transaction.ToAccount != null)
                        moneyInMap[transaction.ToAccount.Id] = moneyInMap[transaction.ToAccount.Id] + transaction.Amount;
                }

                //return the accounts with the proper actual amounts
                return accounts.Select(a => new AccountDTOOut
                {
                    Id = a.Id,
                    Name = a.Name,
                    InitialAmount = a.InitialAmount,
                    ActualAmount = a.InitialAmount + moneyInMap[a.Id] - moneyOutMap[a.Id],
                    IsActive = a.IsActive,
                    Description = a.Description,
                    AccountType = new AccountTypeDTOOut
                    {
                        Id = a.AccountType.Id,
                        Name = a.AccountType.Name
                    }
                });
            }

            return new List<AccountDTOOut>(0);
        }

        public AccountDTOOut CreateAccount(Guid userId, AccountCreationDTOIn account)
        {
            //account creation data validation
            var validationResults = Utils.Utils.ValidateDTO(account);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //check if the account type exists
            var accountTypeQuery = from at in _walletManagerContext.AccountTypes
                                   where at.Id == account.AccountTypeId
                                   select at;

            //check if the user exists
            var user = GetUser(userId);

            if (accountTypeQuery.Any() && user != null)
            {
                var accountType = accountTypeQuery.First();

                //create the account
                var accountToInsert = new Account
                {
                    User = user,
                    InitialAmount = account.InitialAmount.GetValueOrDefault(),
                    Name = account.Name,
                    AccountType = accountType,
                    IsActive = account.IsActive.GetValueOrDefault(),
                    Description = account.Description
                };

                _walletManagerContext.Add(accountToInsert);

                //commit changes in the db
                _walletManagerContext.SaveChanges();

                //return the created account data
                return new AccountDTOOut
                {
                    Id = accountToInsert.Id,
                    Name = accountToInsert.Name,
                    Description = accountToInsert.Description,
                    InitialAmount = accountToInsert.InitialAmount,
                    ActualAmount = accountToInsert.InitialAmount,
                    IsActive = accountToInsert.IsActive,
                    AccountType = new AccountTypeDTOOut
                    {
                        Id = accountType.Id,
                        Name = accountType.Name
                    }
                };
            }
            else
                throw new IncorrectDataException(Utils.Utils.SerializeSingleError("AccountTypeId", "The account type ID " + account.AccountTypeId + " was not found"));
        }

        public AccountDTOOut UpdateAccount(Guid userId, Guid accountId, AccountUpdateDTOIn account)
        {
            //account update data validation
            var validationResults = Utils.Utils.ValidateDTO(account);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //check if the user owns the account
            var accountToUpdateQuery = from acc in _walletManagerContext.Accounts
                                       where acc.Id == accountId
                                       && acc.User.Id == userId
                                       select acc;

            if(accountToUpdateQuery.Any())
            {
                //check if the provided account type exists
                var accountTypeQuery = from accountType in _walletManagerContext.AccountTypes
                                       where accountType.Id == account.AccountTypeId
                                       select accountType;

                if(accountTypeQuery.Any())
                {
                    var accountType = accountTypeQuery.First();

                    //update the account
                    var accountToUpdate = accountToUpdateQuery.First();
                    accountToUpdate.AccountType = accountType;
                    accountToUpdate.Name = account.Name;
                    accountToUpdate.Description = account.Description;
                    accountToUpdate.IsActive = account.IsActive.GetValueOrDefault();
                    accountToUpdate.InitialAmount = account.InitialAmount.GetValueOrDefault();
                    accountToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                    //commit changes in the db
                    _walletManagerContext.SaveChanges();

                    //return the updated account data
                    return GetAccount(accountId, userId);
                }
                else
                    throw new IncorrectDataException((Utils.Utils.SerializeSingleError("AccountTypeId", "The account type ID " + account.AccountTypeId + " was not found")));
            }
            else
                throw new ElementNotFoundException();
        }

        public void DeleteAccount(Guid userId, Guid accountId)
        {
            var accountQuery = from account in _walletManagerContext.Accounts
                               where account.Id == accountId
                               && account.User.Id == userId
                               select account;

            if (accountQuery.Any())
            {
                //before deleting the account, every transaction from or to it has to be deleted
                var accountTransactionsQuery = from transaction in _walletManagerContext.Transactions
                                               where (transaction.FromAccount != null && transaction.FromAccount.Id == accountId)
                                               || (transaction.ToAccount != null && transaction.ToAccount.Id == accountId)
                                               select transaction;

                foreach(var transaction in accountTransactionsQuery.ToList())
                    _walletManagerContext.Transactions.Remove(transaction);

                //finally, delete the account too
                _walletManagerContext.Accounts.Remove(accountQuery.First());

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
