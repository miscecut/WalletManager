using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.AccountType;
using Misce.WalletManager.DTO.Enums;
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

        public IEnumerable<AccountWithAmountHistoryDTOOut> GetAccountsWithAmountHistory(Guid userId, bool? active = null, GroupByPeriod groupByPeriod = GroupByPeriod.MONTH)
        {
            //get the user accounts
            var accountsQuery = from account in _walletManagerContext.Accounts
                                where account.User.Id == userId
                                select account;
            //apply filters
            if (active != null)
                accountsQuery.Where(a => a.IsActive == active);

            var accounts = accountsQuery.Include(a => a.AccountType).ToList();

            //get all the user's transactions
            var transactions = new TransactionService(_walletManagerContext).GetTransactions(userId, int.MaxValue, 0);

            //create the amounts map: ACCOUNT ID => list of maps DateTime => (profit - loss on that account)
            var profitLossMap = new Dictionary<Guid, IDictionary<DateTime, decimal>>();
            foreach (var account in accounts)
                profitLossMap[account.Id] = new Dictionary<DateTime, decimal>();

            //fill the profit/loss map
            foreach(var transaction in transactions)
            {
                var dateTimeForMap = DateTimeUtils.GetDateTimeForAmountHistory(transaction.DateTime, groupByPeriod);
                var fromAccountId = transaction.FromAccount?.Id;
                var toAccountId = transaction.ToAccount?.Id;

                //sign the loss, if there is
                if(fromAccountId.HasValue)
                {
                    //init amount if not present
                    if (!profitLossMap[fromAccountId.Value].ContainsKey(dateTimeForMap))
                        profitLossMap[fromAccountId.Value][dateTimeForMap] = 0;
                    //subtract the amount
                    profitLossMap[fromAccountId.Value][dateTimeForMap] = profitLossMap[fromAccountId.Value][dateTimeForMap] - transaction.Amount;
                }

                //sign the profit, if there is
                if (toAccountId.HasValue)
                {
                    //init amount if not present
                    if (!profitLossMap[toAccountId.Value].ContainsKey(dateTimeForMap))
                        profitLossMap[toAccountId.Value][dateTimeForMap] = 0;
                    //add the amount
                    profitLossMap[toAccountId.Value][dateTimeForMap] = profitLossMap[toAccountId.Value][dateTimeForMap] + transaction.Amount;
                }
            }

            //this map will contain ACCOUNT ID => map of DateTime => amount of the account at that date
            var accountHistoryMap = new Dictionary<Guid, Dictionary<DateTime, decimal>>();
            //this list will be the response
            var amountHistoryList = new List<AccountWithAmountHistoryDTOOut>(accounts.Count());
            //for each account, starting from its creation date, for every period, sign the actual amount
            foreach (var account in accounts)
            {
                //retrieve the account's profit/loss map
                var accountAmountMap = profitLossMap[account.Id];
                //init the accountHistoryMap
                accountHistoryMap[account.Id] = new Dictionary<DateTime, decimal>();
                //start from the date the account was created (OR THE DATE OF THE VERY FIRST TRANSACTION) and finish in the period which comprehends today
                var oldestTransactionDate = DateTimeUtils.GetOldestStartingDateForAccount(transactions.Where(t => (t.FromAccount != null && t.FromAccount.Id == account.Id) || (t.ToAccount != null && t.ToAccount.Id == account.Id)), account.CreatedDateTime);
                var startingDate = DateTimeUtils.GetDateTimeForAmountHistory(oldestTransactionDate, groupByPeriod);
                var endingDate = DateTimeUtils.GetDateTimeForAmountHistory(DateTime.Now, groupByPeriod);
                //sign the initial amount
                var amount = account.InitialAmount;

                //cicle through every date from start to end (jumping by week or day or month etc.)
                for(var date = startingDate; date <= endingDate; date = DateTimeUtils.GetNextValue(date, groupByPeriod))
                {
                    //retrieve the profit - loss amount of that period
                    if (!accountAmountMap.ContainsKey(date))
                        accountAmountMap[date] = 0;
                    var transactionsIntheCurrentPeriod = accountAmountMap[date];
                    //update the current amount
                    amount += transactionsIntheCurrentPeriod;
                    //and sign it in the new map
                    accountHistoryMap[account.Id][date] = amount;
                }

                //once the account amount map is filled, create the objects to give in response
                amountHistoryList.Add(new AccountWithAmountHistoryDTOOut
                {
                    Id = account.Id,
                    IsActive = account.IsActive,
                    Name = account.Name,
                    ActualAmount = amount,
                    InitialAmount = account.InitialAmount,
                    Description = account.Description,
                    AccountAmountHistory = accountHistoryMap[account.Id].Keys.ToList().Select(date => new AccountAmountHistory
                    {
                        AtDate = ConvertToString(date, groupByPeriod),
                        Amount = accountHistoryMap[account.Id][date]
                    }).ToArray(),
                    AccountType = new AccountTypeDTOOut
                    {
                        Id = account.AccountType.Id,
                        Name = account.AccountType.Name,
                    }
                });
            }

            return amountHistoryList;
        }

        #endregion

        #region Private Methods

        private string ConvertToString(DateTime dateTime, GroupByPeriod groupByPeriod)
        {
            if (groupByPeriod == GroupByPeriod.MONTH)
                return $"{GetMonthName(dateTime.Month)} {dateTime.Year}";
            if(groupByPeriod == GroupByPeriod.YEAR)
                return $"{dateTime.Year}";
            if(groupByPeriod == GroupByPeriod.DAY)
                return $"{dateTime.Day} {GetMonthName(dateTime.Month)} {dateTime.Year}";
            return dateTime.ToString();
        }

        private string GetMonthName(int month)
        {
            if (month == 1)
                return "JAN";
            if (month == 2)
                return "FEB";
            if (month == 3)
                return "MAR";
            if (month == 4)
                return "APR";
            if (month == 5)
                return "MAY";
            if (month == 6)
                return "JUN";
            if (month == 7)
                return "JUL";
            if (month == 8)
                return "AUG";
            if (month == 9)
                return "SEP";
            if (month == 10)
                return "OCT";
            if (month == 11)
                return "NOV";
            return "DIC";
        }

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
