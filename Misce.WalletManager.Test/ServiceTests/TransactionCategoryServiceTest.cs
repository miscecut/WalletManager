using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionCategoryServiceTest
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
            _dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE", _saddamId, _misceId, _svetlanaId);
        }

        [TestMethod]
        public void TestGetTransactionCategories()
        {
            //initialize the services
            var transactionCateogryService = new TransactionCategoryService(_dbContext);

            //verify that svetlana has 3 transaction categories
            var svetlanaTransactionCategories = transactionCateogryService.GetTransactionCategories(_svetlanaId);
            Assert.AreEqual(svetlanaTransactionCategories.Count(), 3);

            //verify that misce has 2 transaction categories, and one of them is elettronica
            var misceTransactionCategories = transactionCateogryService.GetTransactionCategories(_misceId);
            Assert.AreEqual(misceTransactionCategories.Count(), 2);
            Assert.AreEqual(misceTransactionCategories.Where(tc => tc.Name == "Elettronica").Count(), 1);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingCreateTransactionCategory()
        {
            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(_dbContext);

            //try to create a transaction category with no name, this should throw an exception
            var saddamBikes = new TransactionCategoryCreationDTOIn
            {
                Description = "I ride bikes sometimes"
            };
            transactionCategoryService.CreateTransactionCategory(_saddamId, saddamBikes);
        }

        [TestMethod]
        public void TestCreateTransactionCategory()
        {
            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(_dbContext);

            //create a new transaction category for saddam
            var saddamBikes = new TransactionCategoryCreationDTOIn
            {
                Name = "Bikes",
                Description = "I ride bikes sometimes"
            };
            var createdSaddamBikes = transactionCategoryService.CreateTransactionCategory(_saddamId, saddamBikes);

            //verify it was created
            Assert.IsNotNull(createdSaddamBikes);

            //try to retrieve it and verify it has the correct fields
            createdSaddamBikes = transactionCategoryService.GetTransactionCategory(_saddamId, createdSaddamBikes.Id);
            Assert.IsNotNull(createdSaddamBikes);
            Assert.IsFalse(string.IsNullOrEmpty(createdSaddamBikes.Description));
            Assert.AreEqual(createdSaddamBikes.Name, "Bikes");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingUpdateTransactionCategory()
        {
            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(_dbContext);

            //get misce's troie transaction category, which has no description
            var misceWhores = transactionCategoryService.GetTransactionCategories(_misceId).Where(tc => tc.Name == "Troie").First();

            //try to update it with a very long name, this should throw an exception and the update should fail
            var misceWhoresUpdate = new TransactionCategoryUpdateDTOIn
            {
                Name = "alalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalal"
            };
            transactionCategoryService.UpdateTransactionCategory(_misceId, misceWhores.Id, misceWhoresUpdate);
        }

        [TestMethod]
        public void TestUpdateTransactionCategory()
        {
            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(_dbContext);

            //get misce's troie transaction category, which has no description
            var misceWhores = transactionCategoryService.GetTransactionCategories(_misceId).Where(tc => tc.Name == "Troie").First();
            Assert.IsTrue(string.IsNullOrEmpty(misceWhores.Description));

            //and update it with a description
            var misceWhoresUpdate = new TransactionCategoryUpdateDTOIn
            {
                Name = misceWhores.Name,
                Description = "Un uomo deve pur guzzare"
            };
            transactionCategoryService.UpdateTransactionCategory(_misceId, misceWhores.Id, misceWhoresUpdate);

            //verify that the transaction category troie has now a description and hasn't changed name
            misceWhores = transactionCategoryService.GetTransactionCategory(_misceId, misceWhores.Id);
            Assert.IsNotNull(misceWhores);
            Assert.AreEqual(misceWhores.Description, "Un uomo deve pur guzzare");
            Assert.AreEqual(misceWhores.Name, "Troie");
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestFailingDeleteTransactionCategory()
        {
            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(_dbContext);
            var transactionSubCategoryService = new TransactionSubCategoryService(_dbContext);

            //get the saddam's bombs category
            var saddamFood = transactionCategoryService.GetTransactionCategories(_saddamId).Where(tc => tc.Name == "Bombs").FirstOrDefault();
            Assert.IsNotNull(saddamFood);

            //and try to delete it with svetlana's account
            transactionCategoryService.DeleteTransactionCategory(_svetlanaId, saddamFood.Id);

            //the food category was not deleted
            Assert.IsNotNull(transactionCategoryService.GetTransactionCategory(_saddamId, saddamFood.Id));
        }

        [TestMethod]
        public void TestDeleteTransactionCategory()
        {
            //initialize the services
            var transactionService = new TransactionService(_dbContext);
            var transactionCategoryService = new TransactionCategoryService(_dbContext);
            var transactionSubCategoryService = new TransactionSubCategoryService(_dbContext);

            //get the saddam's food category
            var saddamFood = transactionCategoryService.GetTransactionCategories(_saddamId).Where(tc => tc.Name == "Food").FirstOrDefault();
            Assert.IsNotNull(saddamFood);

            //and delete it
            transactionCategoryService.DeleteTransactionCategory(_saddamId, saddamFood.Id);

            //verify that the transaction category food was succesfully deleted
            Assert.IsNull(transactionCategoryService.GetTransactionCategory(_saddamId, saddamFood.Id));
            Assert.IsFalse(transactionSubCategoryService.GetTransactionSubCategories(_saddamId, transactionCategoryId: saddamFood.Id).Any());

            //and also the food subcategory
            Assert.IsFalse(transactionSubCategoryService.GetTransactionSubCategories(_saddamId, transactionCategoryId: saddamFood.Id).Any());

            //and verify that there are now no transactions with the subcategory food
            Assert.IsFalse(transactionService.GetTransactions(_saddamId, 100, 0, categoryId: saddamFood.Id).Any());
        }
    }
}
