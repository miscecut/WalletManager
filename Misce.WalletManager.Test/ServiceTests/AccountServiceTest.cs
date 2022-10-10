using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class AccountServiceTest
    {
        private WalletManagerContext _dbContext = null!;
        private Guid _misceId;
        private Guid _saddamId;
        private Guid _svetlanaId;

        [TestInitialize]
        public void Initialize()
        {
            _misceId = Guid.NewGuid();
            _saddamId = Guid.NewGuid();
            _svetlanaId = Guid.NewGuid();
            _dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_SERVICE", _saddamId, _misceId, _svetlanaId);
        }

        [TestMethod]
        public void TestGetAccounts()
        {
            //initialize the services
            var accountService = new AccountService(_dbContext);

            //get the 3 users' accounts
            var misceAccounts = accountService.GetAccounts(_misceId);
            var saddamAccounts = accountService.GetAccounts(_saddamId);
            var svetlanaAccounts = accountService.GetAccounts(_svetlanaId);

            Assert.AreEqual(misceAccounts.Count(), 2); //miscecut has 2 accounts
            Assert.AreEqual(misceAccounts.Where(a => a.Name == "Banco BPM").Count(), 1); //miscecut has a bank account named "Banco BPM"

            Assert.AreEqual(saddamAccounts.Count(), 1); //saddam has 1 account
            Assert.AreEqual(saddamAccounts.Where(a => a.Name == "Banco BPM").Count(), 0); //saddam doesn't see misce's account
            Assert.AreEqual(saddamAccounts.First().ActualAmount, 313M);
            Assert.AreEqual(saddamAccounts.First().AccountType.Name, "Bank account");

            Assert.AreEqual(svetlanaAccounts.Count(), 2); //svetlana has 2 accounts
            Assert.AreEqual(svetlanaAccounts.Where(a => a.AccountType.Name == "Cash").Count(), 1); //svetlana has an account with type "Soldi"

            //METHOD WITH QUERY PARAMETERS

            var misceActiveAccounts = accountService.GetAccounts(_misceId, active: true);

            Assert.AreEqual(misceActiveAccounts.Count(), 1); //miscecut has 1 active account
            Assert.AreEqual(misceActiveAccounts.Where(a => a.Name == "Contanti").Count(), 1);
        }

        [TestMethod]
        public void TestGetAccount()
        {
            //initialize the services
            var accountService = new AccountService(_dbContext);

            //get misce's cash account's id
            var misceCashAccountGuid = accountService
                .GetAccounts(_misceId)
                .Where(a => a.Name == "Contanti")
                .Select(a => a.Id)
                .First();

            //try to get misce's account with misce's account and saddam's account
            var misceCashAccount = accountService.GetAccount(misceCashAccountGuid, _misceId);
            var saddamGoAway = accountService.GetAccount(misceCashAccountGuid, _saddamId);

            Assert.IsNotNull(misceCashAccount);
            Assert.AreEqual(misceCashAccount.ActualAmount, 9.01M);
            Assert.IsNull(saddamGoAway);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingCreateAccount()
        {
            //initialize the services
            var accountService = new AccountService(_dbContext);

            //try to create a new account with a not valid account type id
            var newSvetlanalAccount = new AccountCreationDTOIn
            {
                InitialAmount = 1000,
                Name = "Buoni pasto",
                IsActive = true,
                AccountTypeId = Guid.NewGuid()
            };
            accountService.CreateAccount(_svetlanaId, newSvetlanalAccount);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            //initialize the services
            var accountTypeService = new AccountTypeService(_dbContext);
            var accountService = new AccountService(_dbContext);

            //get the account types
            var accountTypes = accountTypeService.GetAccountTypes();

            //create a new account for svetlana
            var newSvetlanalAccount = new AccountCreationDTOIn
            {
                InitialAmount = 1000,
                Name = "Buoni pasto",
                IsActive = true,
                AccountTypeId = accountTypes.Where(at => at.Name == "Bank account").Select(at => at.Id).First()
            };
            var newAccount = accountService.CreateAccount(_svetlanaId, newSvetlanalAccount);

            //verify that the account now exists with the correct name and initial amount
            var createdSvetlanaAccount = accountService.GetAccount(newAccount.Id, _svetlanaId);
            Assert.IsNotNull(createdSvetlanaAccount);
            Assert.AreEqual(createdSvetlanaAccount.Name, "Buoni pasto");
            Assert.AreEqual(createdSvetlanaAccount.ActualAmount, 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingUpdateAccount1()
        {
            //initialize the services
            var accountTypeService = new AccountTypeService(_dbContext);
            var accountService = new AccountService(_dbContext);

            //get svetlana's sex bank account, with no description
            var svetlanaBankAccount = accountService.GetAccounts(_svetlanaId, active: true).Where(a => a.Name == "Sex Bank").First();
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
            accountService.UpdateAccount(_svetlanaId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestFailingUpdateAccount2()
        {
            //initialize the services
            var accountTypeService = new AccountTypeService(_dbContext);
            var accountService = new AccountService(_dbContext);

            //get svetlana's sex bank account, with no description
            var svetlanaBankAccount = accountService.GetAccounts(_svetlanaId, active: true).Where(a => a.Name == "Sex Bank").First();
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
            accountService.UpdateAccount(_misceId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            //initialize the services
            var accountService = new AccountService(_dbContext);
            var accountTypeService = new AccountTypeService(_dbContext);

            //get svetlana's sex bank account, with no description
            var svetlanaBankAccount = accountService.GetAccounts(_svetlanaId, active: true).Where(a => a.Name == "Sex Bank").First();
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
            var updatedAccountId = accountService.UpdateAccount(_svetlanaId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);

            //verify the changes in the updated account
            svetlanaBankAccount = accountService.GetAccount(updatedAccountId.Id, _svetlanaId);
            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sexxx Bank");
            Assert.AreEqual(svetlanaBankAccount.Description, "banca di sesso");
            Assert.IsTrue(svetlanaBankAccount.IsActive);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestFailingDeleteAccount()
        {
            //initialize the services
            var accountService = new AccountService(_dbContext);
            var accountTypeService = new AccountTypeService(_dbContext);

            //get saddam's bank account
            var bankAccountType = accountTypeService.GetAccountTypes().Where(at => at.Name == "Bank account").First();
            var saddamBankAllah = accountService.GetAccounts(_saddamId, accountTypeId: bankAccountType.Id).Where(a => a.Name == "Banco Allah").First();
            Assert.IsNotNull(saddamBankAllah);

            //try to delete saddam's account with svetlana's id, this will trigger the exception and fail
            accountService.DeleteAccount(_svetlanaId, saddamBankAllah.Id);
        }

        [TestMethod]
        public void TestDeleteAccount()
        {
            //initialize the services
            var accountService = new AccountService(_dbContext);
            var accountTypeService = new AccountTypeService(_dbContext);
            var transactionService = new TransactionService(_dbContext);

            //get saddam's bank account
            var saddamBankAllah = accountService.GetAccounts(_saddamId).Where(a => a.Name == "Banco Allah").First();
            Assert.IsNotNull(saddamBankAllah);

            //and delete it
            accountService.DeleteAccount(_saddamId, saddamBankAllah.Id);

            //verify that the account was succesfully deleted and there are none left
            Assert.IsFalse(accountService.GetAccounts(_saddamId).Any());
            //verify that, since every transaction was under the deleted account, there are none left
            Assert.IsFalse(transactionService.GetTransactions(_saddamId, 100, 0).Any());
        }
    }
}