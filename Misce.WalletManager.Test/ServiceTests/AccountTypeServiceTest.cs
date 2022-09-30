using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class AccountTypeServiceTest
    {
        private WalletManagerContext _dbContext = null!;

        [TestInitialize]
        public void Initialize()
        {
            _dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_TYPE_SERVICE", Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        }

        [TestMethod]
        public void TestGetAccountTypes()
        {
            var accountTypeService = new AccountTypeService(_dbContext);

            var accountTypes = accountTypeService.GetAccountTypes();

            Assert.AreEqual(accountTypes.Count(), 2);
            Assert.IsTrue(accountTypes.Where(at => at.Name == "Cash").Any());
            Assert.IsTrue(accountTypes.Where(at => at.Name == "Bank account").Any());
        }
    }
}
