using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
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

        public Guid CreateAccount(Guid userId, AccountCreationDTOIn account)
        {
            var accountTypeQuery = from at in _walletManagerContext.AccountTypes
                                   where at.Id == account.AccountTypeId
                                   select at;

            var user = GetUser(userId);

            if (accountTypeQuery.Any() && user != null)
            {
                var accountToInsert = new Account
                {
                    User = user,
                    InitialAmount = account.InitialAmount,
                    Name = account.Name,
                    AccountType = accountTypeQuery.First(),
                    IsActive = account.IncludeInTotal
                };

                _walletManagerContext.Add(accountToInsert);
                _walletManagerContext.SaveChanges();

                return accountToInsert.Id;
            }

            throw new InvalidDataException("The provided application type is not valid!");
        }

        public AccountDTOOut? GetAccount(Guid id, Guid userId)
        {
            var query = from a in _walletManagerContext.Accounts
                        where a.Id == id
                        && a.User.Id == userId
                        select a;

            if (query.Any())
            {
                var account = query.First();

                var accountAmountQuery = from t in _walletManagerContext.Transactions
                                         where (t.FromAccount != null && t.FromAccount.Id == id)
                                         || (t.ToAccount != null && t.ToAccount.Id == id)
                                         select t;

                var moneyIn = accountAmountQuery
                    .Where(t => t.ToAccount != null && t.ToAccount.Id == id)
                    .Select(t => t.Amount)
                    .Sum();

                var moneyOut = accountAmountQuery
                    .Where(t => t.FromAccount != null && t.FromAccount.Id == id)
                    .Select(t => t.Amount)
                    .Sum();

                var actualAccountAmount = account.InitialAmount + moneyIn - moneyOut;

                return new AccountDTOOut
                {
                    Id = account.Id,
                    Name = account.Name,
                    Type = account.AccountType.Name,
                    Amount = actualAccountAmount,
                    IsIncludedInTotal = account.IsActive,
                    Description = account.Description
                };
            }

            return null;
        }

        public IEnumerable<AccountDTOOut> GetAccounts(Guid userId, bool? active = null)
        {
            var query = from a in _walletManagerContext.Accounts
                        where a.User.Id == userId
                        select a;

            if (active != null)
                query = query.Where(a => a.IsActive == active);

            if(query.Any())
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
                    if(transaction.FromAccount != null)
                        moneyOutMap[transaction.FromAccount.Id] = moneyOutMap[transaction.FromAccount.Id] + transaction.Amount;
                    if (transaction.ToAccount != null)
                        moneyInMap[transaction.ToAccount.Id] = moneyInMap[transaction.ToAccount.Id] + transaction.Amount;
                }

                //return the accounts with the proper actual amounts
                return accounts.Select(a => new AccountDTOOut
                {
                    Id = a.Id,
                    Name = a.Name,
                    Type = a.AccountType.Name,
                    Amount = a.InitialAmount + moneyInMap[a.Id] - moneyOutMap[a.Id],
                    IsIncludedInTotal = a.IsActive
                });
            }

            return new List<AccountDTOOut>(0);
        }

        public Guid UpdateAccount(Guid userId, Guid accountId, AccountUpdateDTOIn account)
        {
            var accountToUpdateQuery = from acc in _walletManagerContext.Accounts
                                       where acc.Id == accountId
                                       && acc.User.Id == userId
                                       select acc;

            if(accountToUpdateQuery.Any())
            {
                var accountTypeQuery = from accountType in _walletManagerContext.AccountTypes
                                       where accountType.Id == account.AccountTypeId
                                       select accountType;

                if(accountTypeQuery.Any())
                {
                    var accountToUpdate = accountToUpdateQuery.First();
                    accountToUpdate.AccountType = accountTypeQuery.First();
                    accountToUpdate.Name = account.Name;
                    accountToUpdate.Description = account.Description;
                    accountToUpdate.IsActive = account.IsActive;
                    accountToUpdate.InitialAmount = account.InitialAmount;
                    accountToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                    _walletManagerContext.SaveChanges();

                    return accountToUpdate.Id;
                }

                throw new InvalidDataException("The requested account type was not found");
            }

            throw new InvalidDataException("The requested account was not found");
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
