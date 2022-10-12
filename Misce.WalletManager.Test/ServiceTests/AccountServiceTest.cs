using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.Account;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class AccountServiceTest
    {
        [TestMethod]
        public void TestGetAccounts()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_1", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountService = new AccountService(dbContext);
            var accountTypeService = new AccountTypeService(dbContext);

            //get the bank account account type
            var bankAccountType = accountTypeService.GetAccountTypes().Where(at => at.Name == "Bank account").First();
            Assert.IsNotNull(bankAccountType);

            //get the 3 users' accounts
            var misceAccounts = accountService.GetAccounts(misceId);
            var saddamAccounts = accountService.GetAccounts(saddamId);
            var svetlanaAccounts = accountService.GetAccounts(svetlanaId);

            Assert.AreEqual(misceAccounts.Count(), 2); //miscecut has 2 accounts
            Assert.AreEqual(misceAccounts.Where(a => a.Name == "Banco BPM").Count(), 1); //miscecut has a bank account named "Banco BPM"

            Assert.AreEqual(saddamAccounts.Count(), 1); //saddam has 1 account
            Assert.AreEqual(saddamAccounts.Where(a => a.Name == "Banco BPM").Count(), 0); //saddam doesn't see misce's account
            Assert.AreEqual(saddamAccounts.First().ActualAmount, 313M);
            Assert.AreEqual(saddamAccounts.First().AccountType.Name, "Bank account");

            Assert.AreEqual(svetlanaAccounts.Count(), 2); //svetlana has 2 accounts
            Assert.AreEqual(svetlanaAccounts.Where(a => a.AccountType.Name == "Cash").Count(), 1); //svetlana has an account with type "Soldi"

            //with active query parameter
            var misceActiveAccounts = accountService.GetAccounts(misceId, active: true);

            Assert.AreEqual(misceActiveAccounts.Count(), 1); //miscecut has 1 active account
            Assert.AreEqual(misceActiveAccounts.Where(a => a.Name == "Contanti").Count(), 1);

            //with account type id parameter
            var svetlanaCashAccounts = accountService.GetAccounts(svetlanaId, accountTypeId: bankAccountType.Id);

            Assert.AreEqual(svetlanaCashAccounts.Count(), 1);
        }

        [TestMethod]
        public void TestGetAccount()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_2", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountService = new AccountService(dbContext);

            //get misce's cash account's id
            var misceCashAccountGuid = accountService
                .GetAccounts(misceId)
                .Where(a => a.Name == "Contanti")
                .Select(a => a.Id)
                .First();

            //try to get misce's account with misce's account and saddam's account
            var misceCashAccount = accountService.GetAccount(misceCashAccountGuid, misceId);
            var saddamGoAway = accountService.GetAccount(misceCashAccountGuid, saddamId);

            Assert.IsNotNull(misceCashAccount);
            Assert.AreEqual(misceCashAccount.ActualAmount, 9.01M);
            Assert.IsNotNull(misceCashAccount.AccountType);
            Assert.AreEqual(misceCashAccount.AccountType.Name, "Cash");
            Assert.IsNull(saddamGoAway);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingCreateAccount()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_3", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountService = new AccountService(dbContext);

            //try to create a new account with a not valid account type id
            var newSvetlanalAccount = new AccountCreationDTOIn
            {
                InitialAmount = 1000,
                Name = "Buoni pasto",
                IsActive = true,
                AccountTypeId = Guid.NewGuid()
            };
            accountService.CreateAccount(svetlanaId, newSvetlanalAccount);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_4", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountTypeService = new AccountTypeService(dbContext);
            var accountService = new AccountService(dbContext);

            //get the account types
            var accountTypes = accountTypeService.GetAccountTypes();

            //create a new account for svetlana
            var newSvetlanalAccount = new AccountCreationDTOIn
            {
                InitialAmount = 1000,
                Name = "Buoni pasto",
                Description = "io putana",
                IsActive = true,
                AccountTypeId = accountTypes.Where(at => at.Name == "Bank account").Select(at => at.Id).First()
            };
            var newAccount = accountService.CreateAccount(svetlanaId, newSvetlanalAccount);

            //verify that the account now exists with the correct fields
            var createdSvetlanaAccount = accountService.GetAccount(newAccount.Id, svetlanaId);
            Assert.IsNotNull(createdSvetlanaAccount);
            Assert.AreEqual(createdSvetlanaAccount.Name, "Buoni pasto");
            Assert.AreEqual(createdSvetlanaAccount.ActualAmount, 1000);
            Assert.IsTrue(createdSvetlanaAccount.IsActive);
            Assert.AreEqual(createdSvetlanaAccount.Description, "io putana");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingUpdateAccount1()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_5", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountTypeService = new AccountTypeService(dbContext);
            var accountService = new AccountService(dbContext);

            //get svetlana's sex bank account, with no description
            var svetlanaBankAccount = accountService.GetAccounts(svetlanaId, active: true).Where(a => a.Name == "Sex Bank").First();
            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sex Bank");
            Assert.IsNull(svetlanaBankAccount.Description);

            //try to update the account without a name and a status
            var updatedSvetlanaBankAccount = new AccountUpdateDTOIn
            {
                InitialAmount = svetlanaBankAccount.InitialAmount,
                AccountTypeId = svetlanaBankAccount.AccountType.Id,
                Description = "banca di sesso"
            };
            accountService.UpdateAccount(svetlanaId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestFailingUpdateAccount2()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_6", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountTypeService = new AccountTypeService(dbContext);
            var accountService = new AccountService(dbContext);

            //get svetlana's sex bank account, with no description
            var svetlanaBankAccount = accountService.GetAccounts(svetlanaId, active: true).Where(a => a.Name == "Sex Bank").First();
            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sex Bank");
            Assert.IsNull(svetlanaBankAccount.Description);

            //try to update the account with misce's user
            var updatedSvetlanaBankAccount = new AccountUpdateDTOIn
            {
                InitialAmount = svetlanaBankAccount.InitialAmount,
                Name = "Sexxx Bank",
                IsActive = svetlanaBankAccount.IsActive,
                AccountTypeId = svetlanaBankAccount.AccountType.Id,
                Description = "banca di sesso"
            };
            accountService.UpdateAccount(misceId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_7", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountService = new AccountService(dbContext);
            var accountTypeService = new AccountTypeService(dbContext);

            //get svetlana's sex bank account, with no description
            var svetlanaBankAccount = accountService.GetAccounts(svetlanaId, active: true).Where(a => a.Name == "Sex Bank").First();
            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sex Bank");
            Assert.IsNull(svetlanaBankAccount.Description);

            //update the account with a new name and a description
            var updatedSvetlanaBankAccount = new AccountUpdateDTOIn
            {
                InitialAmount = svetlanaBankAccount.InitialAmount,
                Name = "Sexxx Bank",
                IsActive = svetlanaBankAccount.IsActive,
                AccountTypeId = svetlanaBankAccount.AccountType.Id,
                Description = "banca di sesso"
            };
            var updatedAccountId = accountService.UpdateAccount(svetlanaId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);

            //verify the changes in the updated account
            svetlanaBankAccount = accountService.GetAccount(updatedAccountId.Id, svetlanaId);
            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sexxx Bank");
            Assert.AreEqual(svetlanaBankAccount.Description, "banca di sesso");
            Assert.IsTrue(svetlanaBankAccount.IsActive);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestFailingDeleteAccount()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_8", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountService = new AccountService(dbContext);
            var accountTypeService = new AccountTypeService(dbContext);

            //get saddam's bank account
            var bankAccountType = accountTypeService.GetAccountTypes().Where(at => at.Name == "Bank account").First();
            var saddamBankAllah = accountService.GetAccounts(saddamId, accountTypeId: bankAccountType.Id).Where(a => a.Name == "Banco Allah").First();
            Assert.IsNotNull(saddamBankAllah);

            //try to delete saddam's account with svetlana's id, this will trigger the exception and fail
            accountService.DeleteAccount(svetlanaId, saddamBankAllah.Id);
        }

        [TestMethod]
        public void TestDeleteAccount()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE_9", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountService = new AccountService(dbContext);
            var accountTypeService = new AccountTypeService(dbContext);
            var transactionService = new TransactionService(dbContext);

            //get saddam's bank account
            var saddamBankAllah = accountService.GetAccounts(saddamId).Where(a => a.Name == "Banco Allah").First();
            Assert.IsNotNull(saddamBankAllah);

            //and delete it
            accountService.DeleteAccount(saddamId, saddamBankAllah.Id);

            //verify that the account was succesfully deleted and there are none left
            Assert.IsFalse(accountService.GetAccounts(saddamId).Any());
            //verify that, since every transaction was under the deleted account, there are none left
            Assert.IsFalse(transactionService.GetTransactions(saddamId, 100, 0).Any());
        }
    }
}