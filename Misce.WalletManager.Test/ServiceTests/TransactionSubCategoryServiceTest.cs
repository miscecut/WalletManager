using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionSubCategoryServiceTest
    {
        [TestMethod]
        public void TestGetSubCategories()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_SUBCATEGORY_SERVICE_1", saddamId, misceId, svetlanaId);

            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(dbContext);

            //verify that svetlana has 5 transaction subcategories, 3 of which under the vestiti transaction category
            var svetlanaSubCategories = subCategoryService.GetTransactionSubCategories(svetlanaId);
            Assert.AreEqual(svetlanaSubCategories.Count(), 5);
            Assert.AreEqual(svetlanaSubCategories.Where(sc => sc.TransactionCategory.Name == "Vestiti").Count(), 3);
        }

        [TestMethod]
        public void TestCreateSubCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_SUBCATEGORY_SERVICE_2", saddamId, misceId, svetlanaId);

            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(dbContext);
            var categoryService = new TransactionCategoryService(dbContext);

            //get misce's elettronica transaction category
            var misceCategories = categoryService.GetTransactionCategories(misceId);
            var misceElectronics = misceCategories.Where(tc => tc.Name == "Elettronica").First();

            //create the new pc transaction sub category under the elettronica category
            var miscePc = new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = misceElectronics.Id,
                Name = "PC"
            };

            //verify that the transaction subcategory was correctly created
            var createdMiscePc = subCategoryService.CreateTransactionSubCategory(misceId, miscePc);
            Assert.IsNotNull(createdMiscePc);
            Assert.AreEqual(createdMiscePc.Name, "PC");

            //verify that misce has now 3 transaction subcategories
            var misceSubCategories = subCategoryService.GetTransactionSubCategories(misceId);
            Assert.AreEqual(misceSubCategories.Count(), 3);
        }

        [TestMethod]
        public void TestUpdateSubCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_SUBCATEGORY_SERVICE_3", saddamId, misceId, svetlanaId);

            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(dbContext);
            var categoryService = new TransactionCategoryService(dbContext);

            //get misce's electtronica transaction subcategory
            var misceCategories = categoryService.GetTransactionCategories(misceId);
            var misceElectronics = misceCategories.Where(tc => tc.Name == "Elettronica").First();
            var misceElectronicsSub = subCategoryService.GetTransactionSubCategories(misceId).Where(tsc => tsc.Name == "Elettronica").First();

            //and update it with a new name
            var misceElectronicsUpdate = new TransactionSubCategoryUpdateDTOIn
            {
                TransactionCategoryId = misceElectronics.Id,
                Name = "Elettrodomestici"
            };
            subCategoryService.UpdateTransactionSubCategory(misceId, misceElectronicsSub.Id, misceElectronicsUpdate);

            var misceSubCategories = subCategoryService.GetTransactionSubCategories(misceId);
            var updatedMisceElectronicsSub = misceSubCategories.Where(tsc => tsc.Id == misceElectronicsSub.Id).First();

            Assert.AreEqual(updatedMisceElectronicsSub.Name, "Elettrodomestici");
            Assert.IsTrue(string.IsNullOrEmpty(updatedMisceElectronicsSub.Description));
            Assert.IsFalse(misceSubCategories.Where(tsc => tsc.Name == "Elettronica").Any());
        }

        [TestMethod]
        public void TestDeleteSubCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_SUBCATEGORY_SERVICE_4", saddamId, misceId, svetlanaId);

            //initialize the services
            var subCategoryService = new TransactionSubCategoryService(dbContext);

            //get saddam's explosives transaction subcategory, under the bombs transaction category
            var saddamExplosives = subCategoryService.GetTransactionSubCategories(saddamId).Where(tsc => tsc.Name == "Explosives").First();
            Assert.IsNotNull(saddamExplosives);

            //and delete it
            subCategoryService.DeleteTransactionSubCategory(saddamId, saddamExplosives.Id);

            //verify that the transaction subcategory was deleted
            Assert.IsNull(subCategoryService.GetTransactionSubCategory(saddamId, saddamExplosives.Id));

            //verify that saddam now has only 1 transaction subcategory under the bombs category
            Assert.AreEqual(subCategoryService.GetTransactionSubCategories(saddamId, saddamExplosives.TransactionCategory.Id).Count(), 1);
        }
    }
}
