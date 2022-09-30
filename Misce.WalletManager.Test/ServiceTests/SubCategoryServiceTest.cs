using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class SubCategoryServiceTest
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
            var subCategoryService = new SubCategoryService(_dbContext);

            var svetlanaSubCategories = subCategoryService.GetSubCategories(_svetlanaId);

            Assert.AreEqual(svetlanaSubCategories.Count(), 5);
            Assert.AreEqual(svetlanaSubCategories.Where(sc => sc.Category == "Vestiti").Count(), 3);
        }
    }
}
