using Misce.WalletManager.BL.Classes;
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
            var accountService = new AccountService(_dbContext);

            var misceAccounts = accountService.GetAccounts(_misceId);
            var saddamAccounts = accountService.GetAccounts(_saddamId);
            var svetlanaAccounts = accountService.GetAccounts(_svetlanaId);

            Assert.AreEqual(misceAccounts.Count(), 2); //miscecut has 2 accounts
            Assert.AreEqual(misceAccounts.Where(a => a.Name == "Banco BPM").Count(), 1); //miscecut has a bank account named "Banco BPM"

            Assert.AreEqual(saddamAccounts.Count(), 1); //saddam has 1 account
            Assert.AreEqual(saddamAccounts.Where(a => a.Name == "Banco BPM").Count(), 0); //saddam doesn't see misce's account
            Assert.AreEqual(saddamAccounts.First().Amount, 313M);

            Assert.AreEqual(svetlanaAccounts.Count(), 2); //svetlana has 2 accounts
            Assert.AreEqual(svetlanaAccounts.Where(a => a.AccountType.Name == "Cash").Count(), 1); //svetlana has an account with type "Soldi"

            //METHOD WITH QUERY PARAMETERS

            var misceActiveAccounts = accountService.GetAccounts(_misceId, true);

            Assert.AreEqual(misceActiveAccounts.Count(), 1); //miscecut has 1 active account
            Assert.AreEqual(misceActiveAccounts.Where(a => a.Name == "Contanti").Count(), 1);
        }

        [TestMethod]
        public void TestGetAccount()
        {
            var accountService = new AccountService(_dbContext);

            var misceCashAccountGuid = accountService
                .GetAccounts(_misceId)
                .Where(a => a.Name == "Contanti")
                .Select(a => a.Id)
                .First();

            var misceCashAccount = accountService.GetAccount(misceCashAccountGuid, _misceId);
            var saddamGoAway = accountService.GetAccount(misceCashAccountGuid, _saddamId);

            Assert.IsNotNull(misceCashAccount);
            Assert.AreEqual(misceCashAccount.Amount, 9.01M);
            Assert.IsNull(saddamGoAway);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            var accountTypeService = new AccountTypeService(_dbContext);
            var accountService = new AccountService(_dbContext);

            var accountTypes = accountTypeService.GetAccountTypes();

            var newSvetlanalAccount = new AccountCreationDTOIn
            {
                InitialAmount = 1000,
                Name = "Buoni pasto",
                IncludeInTotal = true,
                AccountTypeId = accountTypes.Where(at => at.Name == "Bank account").Select(at => at.Id).First()
            };

            var newAccountId = accountService.CreateAccount(_svetlanaId, newSvetlanalAccount);

            //CREATION CHECK

            var createdSvetlanaAccount = accountService.GetAccount(newAccountId, _svetlanaId);

            Assert.IsNotNull(createdSvetlanaAccount);
            Assert.AreEqual(createdSvetlanaAccount.Name, "Buoni pasto");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException), "The provided application type id is not valid")]
        public void TestFailingCreateAccount()
        {
            var accountService = new AccountService(_dbContext);

            var newSvetlanalAccount = new AccountCreationDTOIn
            {
                InitialAmount = 1000,
                Name = "Buoni pasto",
                IncludeInTotal = true,
                AccountTypeId = Guid.NewGuid()
            };

            accountService.CreateAccount(_svetlanaId, newSvetlanalAccount);
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            var accountService = new AccountService(_dbContext);
            var accountTypeService = new AccountTypeService(_dbContext);

            var svetlanaBankAccount = accountService.GetAccounts(_svetlanaId, true).Where(a => a.Name == "Sex Bank").First();
            var bankType = accountTypeService.GetAccountTypes().Where(at => at.Name == "Bank account").First();

            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sex Bank");
            Assert.IsNull(svetlanaBankAccount.Description);

            var updatedSvetlanaBankAccount = new AccountUpdateDTOIn
            {
                InitialAmount = 3,
                Name = "Sexxx Bank",
                IsActive = svetlanaBankAccount.IsIncludedInTotal,
                AccountTypeId = bankType.Id,
                Description = "banca di sesso"
            };

            var updatedAccountId = accountService.UpdateAccount(_svetlanaId, svetlanaBankAccount.Id, updatedSvetlanaBankAccount);

            svetlanaBankAccount = accountService.GetAccount(updatedAccountId, _svetlanaId);

            Assert.IsNotNull(svetlanaBankAccount);
            Assert.AreEqual(svetlanaBankAccount.Name, "Sexxx Bank");
            Assert.AreEqual(svetlanaBankAccount.Description, "banca di sesso");
            Assert.IsTrue(svetlanaBankAccount.IsIncludedInTotal);
        }
    }
}