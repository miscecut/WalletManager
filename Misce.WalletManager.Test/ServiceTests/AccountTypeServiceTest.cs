using Misce.WalletManager.BL.Classes;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class AccountTypeServiceTest
    {
        [TestMethod]
        public void TestGetAccountTypes()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

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
