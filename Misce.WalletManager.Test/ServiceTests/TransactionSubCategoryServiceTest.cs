using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
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

        [TestMethod]
        public void TestCreateSubCategory()
        {
            var subCategoryService = new TransactionSubCategoryService(_dbContext);
            var categoryService = new TransactionCategoryService(_dbContext);

            var misceCategories = categoryService.GetTransactionCategories(_misceId);
            var misceElectronics = misceCategories.Where(tc => tc.Name == "Elettronica").First();

            var miscePc = new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = misceElectronics.Id,
                Name = "PC"
            };

            var createdMiscePc = subCategoryService.CreateTransactionSubCategory(_misceId, miscePc);

            Assert.IsNotNull(createdMiscePc);
            Assert.AreEqual(createdMiscePc.Name, "PC");

            var misceSubCategories = subCategoryService.GetTransactionSubCategories(_misceId);

            Assert.AreEqual(misceSubCategories.Count(), 3);
        }

        [TestMethod]
        public void TestUpdateSubCategory()
        {
            var subCategoryService = new TransactionSubCategoryService(_dbContext);
            var categoryService = new TransactionCategoryService(_dbContext);

            var misceCategories = categoryService.GetTransactionCategories(_misceId);
            var misceElectronics = misceCategories.Where(tc => tc.Name == "Elettronica").First();
            var misceElectronicsSub = subCategoryService.GetTransactionSubCategories(_misceId).Where(tsc => tsc.Name == "Elettronica").First();

            var misceElectronicsUpdate = new TransactionSubCategoryUpdateDTOIn
            {
                TransactionCategoryId = misceElectronics.Id,
                Name = "Elettrodomestici"
            };

            subCategoryService.UpdateTransactionSubCategory(_misceId, misceElectronicsSub.Id, misceElectronicsUpdate);

            var misceSubCategories = subCategoryService.GetTransactionSubCategories(_misceId);
            var updatedMisceElectronicsSub = misceSubCategories.Where(tsc => tsc.Id == misceElectronicsSub.Id).First();

            Assert.AreEqual(updatedMisceElectronicsSub.Name, "Elettrodomestici");
            Assert.IsTrue(string.IsNullOrEmpty(updatedMisceElectronicsSub.Description));
            Assert.IsFalse(misceSubCategories.Where(tsc => tsc.Name == "Elettronica").Any());
        }
    }
}
