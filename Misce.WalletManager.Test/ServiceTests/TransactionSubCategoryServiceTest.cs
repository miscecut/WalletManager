using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionSubCategoryServiceTest
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
            _dbContext = DbContextGeneration.GenerateDb("TEST_SUBCATEGORY_SERVICE", _saddamId, _misceId, _svetlanaId);
        }

        [TestMethod]
        public void TestGetSubCategories()
        {
            var subCategoryService = new TransactionSubCategoryService(_dbContext);

            var svetlanaSubCategories = subCategoryService.GetTransactionSubCategories(_svetlanaId);

            Assert.AreEqual(svetlanaSubCategories.Count(), 5);
            Assert.AreEqual(svetlanaSubCategories.Where(sc => sc.Category.Name == "Vestiti").Count(), 3);
        }


    }
}
