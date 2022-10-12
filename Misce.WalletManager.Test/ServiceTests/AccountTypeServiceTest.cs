using Misce.WalletManager.BL.Classes;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class AccountTypeServiceTest
    {
        [TestMethod]
        public void TestGetAccountTypes()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_ACCOUNT_TYPE_SERVICE_1", saddamId, misceId, svetlanaId);

            //initialize the services
            var accountTypeService = new AccountTypeService(dbContext);

            //verify there are only 2 account types: cash and bank account
            var accountTypes = accountTypeService.GetAccountTypes();
            Assert.AreEqual(accountTypes.Count(), 2);
            Assert.IsTrue(accountTypes.Where(at => at.Name == "Cash").Any());
            Assert.IsTrue(accountTypes.Where(at => at.Name == "Bank account").Any());
        }
    }
}
