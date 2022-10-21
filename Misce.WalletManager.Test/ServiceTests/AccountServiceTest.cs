using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.DTO.DTO.User;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class AccountServiceTest
    {
        [TestMethod]
        public void TestAccountCreateAndGet()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var accountService = new AccountService(dbContext);
            var accountTypeService = new AccountTypeService(dbContext);
            var transactionService = new TransactionService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "miscecut",
                Password = "GS7fss787f",
                ConfirmPassword = "GS7fss787f"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "nomeacaso",
                Password = "dnvin!IHUIHUIH2",
                ConfirmPassword = "dnvin!IHUIHUIH2"
            });

            //get the bank account account type
            var bankAccountTypeId = accountTypeService.GetAccountTypes().Where(at => at.Name == "Bank account").First().Id;
            var cashAccountTypeId = accountTypeService.GetAccountTypes().Where(at => at.Name == "Cash").First().Id;

            //create the user's inactive bank account
            var createdUserBankAccount = accountService.CreateAccount(user.Id, new AccountCreationDTOIn
            {
                Name = "NY Bank",
                InitialAmount = 500,
                IsActive = false,
                AccountTypeId = bankAccountTypeId
            });
            Assert.IsNotNull(createdUserBankAccount);
            Assert.IsFalse(createdUserBankAccount.IsActive);
            Assert.AreEqual(createdUserBankAccount.Name, "NY Bank");
            Assert.IsTrue(string.IsNullOrEmpty(createdUserBankAccount.Description));
            Assert.AreEqual(createdUserBankAccount.InitialAmount, 500);
            Assert.AreEqual(createdUserBankAccount.ActualAmount, 500);
            Assert.IsNotNull(createdUserBankAccount.AccountType);
            Assert.AreEqual(createdUserBankAccount.AccountType.Name, "Bank account");

            //create the user's wallet
            var createdUserWallet = accountService.CreateAccount(user.Id, new AccountCreationDTOIn
            {
                Name = "Wallet",
                InitialAmount = 14.69M,
                IsActive = true,
                AccountTypeId = cashAccountTypeId,
                Description = "my wallet!"
            });
            Assert.IsNotNull(createdUserWallet);
            Assert.IsTrue(createdUserWallet.IsActive);
            Assert.AreEqual(createdUserWallet.Name, "Wallet");
            Assert.AreEqual(createdUserWallet.Description, "my wallet!");
            Assert.AreEqual(createdUserWallet.InitialAmount, 14.69M);
            Assert.AreEqual(createdUserWallet.ActualAmount, 14.69M);
            Assert.IsNotNull(createdUserWallet.AccountType);
            Assert.AreEqual(createdUserWallet.AccountType.Name, "Cash");

            //create the other user's bank account
            var createdOtherUserBankAccount = accountService.CreateAccount(otherUser.Id, new AccountCreationDTOIn
            {
                Name = "BPM",
                InitialAmount = 12.8M,
                IsActive = true,
                AccountTypeId = bankAccountTypeId
            });
            Assert.IsNotNull(createdOtherUserBankAccount);
            Assert.IsTrue(createdOtherUserBankAccount.IsActive);
            Assert.AreEqual(createdOtherUserBankAccount.Name, "BPM");
            Assert.IsTrue(string.IsNullOrEmpty(createdOtherUserBankAccount.Description));
            Assert.AreEqual(createdOtherUserBankAccount.InitialAmount, 12.8M);
            Assert.AreEqual(createdOtherUserBankAccount.ActualAmount, 12.8M);
            Assert.IsNotNull(createdOtherUserBankAccount.AccountType);
            Assert.AreEqual(createdOtherUserBankAccount.AccountType.Name, "Bank account");

            //get user's accounts
            var userAccounts = accountService.GetAccounts(user.Id);
            Assert.AreEqual(userAccounts.Count(), 2);
            Assert.IsFalse(userAccounts.Where(a => a.Name == "BPM").Any());

            //get user's active accounts
            var userActiveAccounts = accountService.GetAccounts(user.Id, active: true);
            Assert.AreEqual(userActiveAccounts.Count(), 1);
            Assert.IsTrue(userActiveAccounts.Where(a => a.Name == "Wallet").Any());
            Assert.AreEqual(userActiveAccounts.First().ActualAmount, 14.69M);

            //get user's bank accounts
            var userBankAccounts = accountService.GetAccounts(user.Id, accountTypeId: bankAccountTypeId);
            Assert.AreEqual(userBankAccounts.Count(), 1);
            Assert.IsTrue(userBankAccounts.Where(a => a.Name == "NY Bank").Any());

            //create an expense to decrease the total amount of the user's wallet
            transactionService.CreateTransaction(user.Id, new TransactionCreationDTOIn
            {
                FromAccountId = createdUserWallet.Id,
                Amount = 1.19M,
                Title = "Onions",
                DateTime = DateTime.UtcNow
            });
            Assert.AreEqual(accountService.GetAccounts(user.Id, active: true, accountTypeId: cashAccountTypeId).Single().ActualAmount, 13.5M);

            //create a profit to decrease the total amount of the user's wallet
            transactionService.CreateTransaction(user.Id, new TransactionCreationDTOIn
            {
                ToAccountId = createdUserWallet.Id,
                Amount = 20,
                Title = "Random money",
                DateTime = DateTime.UtcNow
            });
            Assert.AreEqual(accountService.GetAccounts(user.Id, active: true, accountTypeId: cashAccountTypeId).Single().ActualAmount, 33.5M);
        }
    }
}