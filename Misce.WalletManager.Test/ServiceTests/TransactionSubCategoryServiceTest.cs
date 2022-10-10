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
            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(_dbContext);

            //verify that svetlana has 5 transaction subcategories, 3 of which under the vestiti transaction category
            var svetlanaSubCategories = subCategoryService.GetTransactionSubCategories(_svetlanaId);
            Assert.AreEqual(svetlanaSubCategories.Count(), 5);
            Assert.AreEqual(svetlanaSubCategories.Where(sc => sc.TransactionCategory.Name == "Vestiti").Count(), 3);
        }

        [TestMethod]
        public void TestCreateSubCategory()
        {
            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(_dbContext);
            var categoryService = new TransactionCategoryService(_dbContext);

            //get misce's elettronica transaction category
            var misceCategories = categoryService.GetTransactionCategories(_misceId);
            var misceElectronics = misceCategories.Where(tc => tc.Name == "Elettronica").First();

            //create the new pc transaction sub category under the elettronica category
            var miscePc = new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = misceElectronics.Id,
                Name = "PC"
            };

            //verify that the transaction subcategory was correctly created
            var createdMiscePc = subCategoryService.CreateTransactionSubCategory(_misceId, miscePc);
            Assert.IsNotNull(createdMiscePc);
            Assert.AreEqual(createdMiscePc.Name, "PC");

            //verify that misce has now 3 transaction subcategories
            var misceSubCategories = subCategoryService.GetTransactionSubCategories(_misceId);
            Assert.AreEqual(misceSubCategories.Count(), 3);
        }

        [TestMethod]
        public void TestUpdateSubCategory()
        {
            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(_dbContext);
            var categoryService = new TransactionCategoryService(_dbContext);

            //get misce's electtronica transaction subcategory
            var misceCategories = categoryService.GetTransactionCategories(_misceId);
            var misceElectronics = misceCategories.Where(tc => tc.Name == "Elettronica").First();
            var misceElectronicsSub = subCategoryService.GetTransactionSubCategories(_misceId).Where(tsc => tsc.Name == "Elettronica").First();

            //and update it with a new name
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

        [TestMethod]
        public void TestDeleteSubCategory()
        {
            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(_dbContext);

            //get saddam's explosives transaction subcategory, under the bombs transaction category
            var saddamExplosives = subCategoryService.GetTransactionSubCategories(_saddamId).Where(tsc => tsc.Name == "Explosives").First();
            Assert.IsNotNull(saddamExplosives);

            //and delete it
            subCategoryService.DeleteTransactionSubCategory(_saddamId, saddamExplosives.Id);

            //verify that the transaction subcategory was deleted
            Assert.IsNull(subCategoryService.GetTransactionSubCategory(_saddamId, saddamExplosives.Id));

            //verify that saddam now has only 1 transaction subcategory under the bombs category
            Assert.AreEqual(subCategoryService.GetTransactionSubCategories(_saddamId, saddamExplosives.TransactionCategory.Id).Count(), 1);
        }
    }
}
