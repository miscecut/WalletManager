using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.TransactionCategory;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionCategoryServiceTest
    {
        [TestMethod]
        public void TestGetTransactionCategories()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_1", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionCateogryService = new TransactionCategoryService(dbContext);

            //verify that svetlana has 3 transaction categories
            var svetlanaTransactionCategories = transactionCateogryService.GetTransactionCategories(svetlanaId);
            Assert.AreEqual(svetlanaTransactionCategories.Count(), 3);

            //verify that misce has 2 transaction categories, and one of them is elettronica
            var misceTransactionCategories = transactionCateogryService.GetTransactionCategories(misceId);
            Assert.AreEqual(misceTransactionCategories.Count(), 2);
            Assert.AreEqual(misceTransactionCategories.Where(tc => tc.Name == "Elettronica").Count(), 1);

            //with expense type query parameter, saddam has 2 expense transaction categories
            var saddamExpenseTransactionCartegories = transactionCateogryService.GetTransactionCategories(saddamId, isExpenseType: true);
            Assert.AreEqual(saddamExpenseTransactionCartegories.Count(), 2);
            Assert.IsFalse(saddamExpenseTransactionCartegories.Where(tc => tc.Name == "Drugs").Any());
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingCreateTransactionCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_2", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //try to create a transaction category with no name, this should throw an exception
            var saddamBikes = new TransactionCategoryCreationDTOIn
            {
                Description = "I ride bikes sometimes"
            };
            transactionCategoryService.CreateTransactionCategory(saddamId, saddamBikes);
        }

        [TestMethod]
        public void TestCreateTransactionCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_3", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create a new transaction category for saddam
            var saddamBikes = new TransactionCategoryCreationDTOIn
            {
                Name = "Bikes",
                Description = "I ride bikes sometimes"
            };
            var createdSaddamBikes = transactionCategoryService.CreateTransactionCategory(saddamId, saddamBikes);

            //verify it was created
            Assert.IsNotNull(createdSaddamBikes);

            //try to retrieve it and verify it has the correct fields
            createdSaddamBikes = transactionCategoryService.GetTransactionCategory(saddamId, createdSaddamBikes.Id);
            Assert.IsNotNull(createdSaddamBikes);
            Assert.IsFalse(string.IsNullOrEmpty(createdSaddamBikes.Description));
            Assert.AreEqual(createdSaddamBikes.Name, "Bikes");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingUpdateTransactionCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_4", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //get misce's troie transaction category, which has no description
            var misceWhores = transactionCategoryService.GetTransactionCategories(misceId).Where(tc => tc.Name == "Troie").First();

            //try to update it with a very long name, this should throw an exception and the update should fail
            var misceWhoresUpdate = new TransactionCategoryUpdateDTOIn
            {
                Name = "alalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalalal"
            };
            transactionCategoryService.UpdateTransactionCategory(misceId, misceWhores.Id, misceWhoresUpdate);
        }

        [TestMethod]
        public void TestUpdateTransactionCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_5", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //get misce's troie transaction category, which has no description
            var misceWhores = transactionCategoryService.GetTransactionCategories(misceId).Where(tc => tc.Name == "Troie").First();
            Assert.IsTrue(string.IsNullOrEmpty(misceWhores.Description));

            //and update it with a description
            var misceWhoresUpdate = new TransactionCategoryUpdateDTOIn
            {
                Name = misceWhores.Name,
                Description = "Un uomo deve pur guzzare"
            };
            transactionCategoryService.UpdateTransactionCategory(misceId, misceWhores.Id, misceWhoresUpdate);

            //verify that the transaction category troie has now a description and hasn't changed name
            misceWhores = transactionCategoryService.GetTransactionCategory(misceId, misceWhores.Id);
            Assert.IsNotNull(misceWhores);
            Assert.AreEqual(misceWhores.Description, "Un uomo deve pur guzzare");
            Assert.AreEqual(misceWhores.Name, "Troie");
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestFailingDeleteTransactionCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_6", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionCategoryService = new TransactionCategoryService(dbContext);
            var transactionSubCategoryService = new TransactionSubCategoryService(dbContext);

            //get the saddam's bombs category
            var saddamFood = transactionCategoryService.GetTransactionCategories(saddamId).Where(tc => tc.Name == "Bombs").FirstOrDefault();
            Assert.IsNotNull(saddamFood);

            //and try to delete it with svetlana's account
            transactionCategoryService.DeleteTransactionCategory(svetlanaId, saddamFood.Id);

            //the food category was not deleted
            Assert.IsNotNull(transactionCategoryService.GetTransactionCategory(saddamId, saddamFood.Id));
        }

        [TestMethod]
        public void TestDeleteTransactionCategory()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_CATEGORY_SERVICE_7", saddamId, misceId, svetlanaId);

            //initialize the services
            var transactionService = new TransactionService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);
            var transactionSubCategoryService = new TransactionSubCategoryService(dbContext);

            //get the saddam's food category
            var saddamFood = transactionCategoryService.GetTransactionCategories(saddamId).Where(tc => tc.Name == "Food").FirstOrDefault();
            Assert.IsNotNull(saddamFood);

            //and delete it
            transactionCategoryService.DeleteTransactionCategory(saddamId, saddamFood.Id);

            //verify that the transaction category food was succesfully deleted
            Assert.IsNull(transactionCategoryService.GetTransactionCategory(saddamId, saddamFood.Id));
            Assert.IsFalse(transactionSubCategoryService.GetTransactionSubCategories(saddamId, transactionCategoryId: saddamFood.Id).Any());

            //and also the food subcategory
            Assert.IsFalse(transactionSubCategoryService.GetTransactionSubCategories(saddamId, transactionCategoryId: saddamFood.Id).Any());

            //and verify that there are now no transactions with the subcategory food
            Assert.IsFalse(transactionService.GetTransactions(saddamId, 100, 0, categoryId: saddamFood.Id).Any());
        }
    }
}
