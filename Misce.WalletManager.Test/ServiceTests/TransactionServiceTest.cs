using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.DTO.DTO;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionServiceTest
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
            _dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_SERVICE", _saddamId, _misceId, _svetlanaId);
        }

        [TestMethod]
        public void TestGetTransactions()
        {
            var transactionService = new TransactionService(_dbContext);

            var saddamTransactions = transactionService.GetTransactions(_saddamId, 8, 0);

            Assert.AreEqual(saddamTransactions.Count(), 4);
            Assert.AreEqual(saddamTransactions.Where(t => t.Amount == 3.5M).Count(), 2);
            Assert.AreEqual(saddamTransactions.Where(t => t.FromAccountName == "Banco Allah").Count(), 3);
            Assert.AreEqual(saddamTransactions.Where(t => t.ToAccountName == "Banco Allah").Count(), 1);
            Assert.AreEqual(saddamTransactions.Where(t => t.SubCategory == "Food").Count(), 2);

            //WITH QUERY PARAMETERS

            var saddamTransactionsOnThe23 = transactionService.GetTransactions(_saddamId, 10, 0, toDateTime: new DateTime(2022, 2, 23, 23, 59, 59));

            Assert.AreEqual(saddamTransactionsOnThe23.Count(), 3);
        }

        [TestMethod]
        public void TestCreateTransaction()
        {
            var transactionService = new TransactionService(_dbContext);
            var accountService = new AccountService(_dbContext);
            var subCategoryService = new SubCategoryService(_dbContext);

            var misceAccounts = accountService.GetAccounts(_misceId);
            var saddamAccounts = accountService.GetAccounts(_saddamId);

            var saddamSubCategories = subCategoryService.GetSubCategories(_saddamId);

            var saddamDrugs = saddamSubCategories.Where(sc => sc.Name == "Drugs").Single();

            var misceCash = misceAccounts.Where(a => a.Name == "Contanti").Single();
            var saddamBankAccount = saddamAccounts.Where(a => a.Name == "Banco Allah").Single();

            //GAIN WITH NO SUBCATEGORY

            var misceGainId = transactionService.CreateTransaction(_misceId, new TransactionDTOIn
            {
                Amount = 50,
                ToAccountId = misceCash.Id,
                DateTime = DateTime.UtcNow,
                Title = "Paghetta"
            });

            Assert.IsNotNull(misceGainId);
            Assert.AreEqual(transactionService.GetTransactions(_misceId, 10, 0, toAccountId: misceCash.Id).Count(), 1);

            //GAIN WITH SUBCATEGORY

            var saddamGainId = transactionService.CreateTransaction(_saddamId, new TransactionDTOIn
            {
                Amount = 69.42M,
                SubCategoryId = saddamDrugs.Id,
                ToAccountId = saddamBankAccount.Id,
                DateTime = DateTime.UtcNow,
                Title = "Weed",
                Description = "Got some money baby",
            });

            Assert.IsNotNull(saddamGainId);
            Assert.AreEqual(transactionService.GetTransactions(_saddamId, 10, 0, subCategoryId: saddamDrugs.Id).Count(), 2);
        }
    }
}
