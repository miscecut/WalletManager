using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.DTO.DTO.User;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionCategoryServiceTest
    {
        [TestMethod]
        public void TestTransactionCategoryCreationAndRetrieving()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //create another user
            var anotherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "faust",
                Password = "adg2aFFFF",
                ConfirmPassword = "adg2aFFFF"
            });

            //create the electronics transaction category
            var createdElectronicsTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Electronics",
                Description = "Electronic stuff",
                IsExpenseType = true
            });
            Assert.IsNotNull(createdElectronicsTransactionCategory);
            Assert.AreEqual(createdElectronicsTransactionCategory.Name, "Electronics");
            Assert.AreEqual(createdElectronicsTransactionCategory.Description, "Electronic stuff");
            Assert.IsTrue(createdElectronicsTransactionCategory.IsExpenseType);

            //create the bills transaction category
            var createdBillsTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Bills",
                IsExpenseType = true
            });
            Assert.IsNotNull(createdBillsTransactionCategory);
            Assert.AreEqual(createdBillsTransactionCategory.Name, "Bills");
            Assert.IsTrue(string.IsNullOrEmpty(createdBillsTransactionCategory.Description));
            Assert.IsTrue(createdBillsTransactionCategory.IsExpenseType);

            //create the bills transaction category
            var createdSalaryTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Salary",
                IsExpenseType = false
            });
            Assert.IsNotNull(createdSalaryTransactionCategory);
            Assert.AreEqual(createdSalaryTransactionCategory.Name, "Salary");
            Assert.IsTrue(string.IsNullOrEmpty(createdSalaryTransactionCategory.Description));
            Assert.IsFalse(createdSalaryTransactionCategory.IsExpenseType);

            //create the dresses transaction category under the other user
            var createdDressesTransactionCategory = transactionCategoryService.CreateTransactionCategory(anotherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Dresses",
                IsExpenseType = true
            });
            Assert.IsNotNull(createdDressesTransactionCategory);
            Assert.AreEqual(createdDressesTransactionCategory.Name, "Dresses");
            Assert.IsTrue(string.IsNullOrEmpty(createdDressesTransactionCategory.Description));
            Assert.IsTrue(createdDressesTransactionCategory.IsExpenseType);

            //get all 3 user's transaction categories, the other user's dresses category must not be seen
            var allTransactionCategories = transactionCategoryService.GetTransactionCategories(user.Id);
            Assert.AreEqual(allTransactionCategories.Count(), 3);
            Assert.AreEqual(allTransactionCategories.Where(tc => tc.IsExpenseType).Count(), 2);
            Assert.AreEqual(allTransactionCategories.Where(tc => tc.Name == "Salary").Count(), 1);

            //get only the profit transaction categories
            var allProfitTransactionCategories = transactionCategoryService.GetTransactionCategories(user.Id, isExpenseType: false);
            Assert.AreEqual(allProfitTransactionCategories.Count(), 1);
            Assert.IsFalse(allProfitTransactionCategories.Where(tc => tc.IsExpenseType).Any());
            Assert.AreEqual(allProfitTransactionCategories.Where(tc => tc.Name == "Salary").Count(), 1);

            //get only the user's transactions which contain "ele" in the name
            var allEleTransactionCategories = transactionCategoryService.GetTransactionCategories(user.Id, name: "ele");
            Assert.AreEqual(allEleTransactionCategories.Count(), 1);
            Assert.AreEqual(allEleTransactionCategories.Where(tc => tc.Name == "Electronics").Count(), 1);
        }

        [TestMethod]
        public void TestTransactionCategoryCreationAndSingleRetrieving()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //create another user
            var anotherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "faust",
                Password = "adg2aFFFF",
                ConfirmPassword = "adg2aFFFF"
            });

            //create the electronics transaction category
            var createdElectronicsTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Electronics",
                Description = "Electronic stuff",
                IsExpenseType = true
            });
            Assert.IsNotNull(createdElectronicsTransactionCategory);
            Assert.AreEqual(createdElectronicsTransactionCategory.Name, "Electronics");
            Assert.AreEqual(createdElectronicsTransactionCategory.Description, "Electronic stuff");
            Assert.IsTrue(createdElectronicsTransactionCategory.IsExpenseType);

            //create the bills transaction category
            var createdSalaryTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Salary",
                IsExpenseType = false
            });
            Assert.IsNotNull(createdSalaryTransactionCategory);
            Assert.AreEqual(createdSalaryTransactionCategory.Name, "Salary");
            Assert.IsTrue(string.IsNullOrEmpty(createdSalaryTransactionCategory.Description));
            Assert.IsFalse(createdSalaryTransactionCategory.IsExpenseType);

            //create the dresses transaction category under the other user
            var createdDressesTransactionCategory = transactionCategoryService.CreateTransactionCategory(anotherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Dresses",
                IsExpenseType = true
            });
            Assert.IsNotNull(createdDressesTransactionCategory);
            Assert.AreEqual(createdDressesTransactionCategory.Name, "Dresses");
            Assert.IsTrue(string.IsNullOrEmpty(createdDressesTransactionCategory.Description));
            Assert.IsTrue(createdDressesTransactionCategory.IsExpenseType);

            //get the salary category with the user id
            var salaryTransactionCategory = transactionCategoryService.GetTransactionCategory(user.Id, createdSalaryTransactionCategory.Id);
            Assert.IsNotNull(salaryTransactionCategory);
            Assert.AreEqual(salaryTransactionCategory.Name, "Salary");
            Assert.IsTrue(string.IsNullOrEmpty(salaryTransactionCategory.Description));
            Assert.IsFalse(salaryTransactionCategory.IsExpenseType);

            //get the other user's dresses category with the user id, this should be impossible
            var dressesTransactionCategory = transactionCategoryService.GetTransactionCategory(user.Id, createdDressesTransactionCategory.Id);
            Assert.IsNull(dressesTransactionCategory);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestUnprovidedNameFailingTransactionCategoryCreation()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //try to create a transactionc category without providing a name
            transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Description = "no name transaction",
                IsExpenseType = false
            });
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestUnprovidedIsExpenseTypeFailingTransactionCategoryCreation()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //try to create a transactionc category without telling if it's an expense or profit category
            transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Transaction"
            });
        }

        [TestMethod]
        public void TestTransactionCategoryCorrectUpdate()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //create the electronics transaction category
            var createdElectronicsTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Electronics",
                Description = "Electronic stuff",
                IsExpenseType = true
            });

            //change name to the category
            var updatedElectronicsTransactionCategory = transactionCategoryService.UpdateTransactionCategory(user.Id, createdElectronicsTransactionCategory.Id, new TransactionCategoryUpdateDTOIn
            {
                Name = "Electronics & Stuff",
                Description = "Electronic stuff",
                IsExpenseType = true
            });
            Assert.IsNotNull(updatedElectronicsTransactionCategory);
            Assert.AreEqual(updatedElectronicsTransactionCategory.Name, "Electronics & Stuff");
            Assert.AreEqual(updatedElectronicsTransactionCategory.Description, "Electronic stuff");
            Assert.IsTrue(updatedElectronicsTransactionCategory.IsExpenseType);

            updatedElectronicsTransactionCategory = transactionCategoryService.GetTransactionCategory(user.Id, updatedElectronicsTransactionCategory.Id);
            Assert.IsNotNull(updatedElectronicsTransactionCategory);
            Assert.AreEqual(updatedElectronicsTransactionCategory.Name, "Electronics & Stuff");
            Assert.AreEqual(updatedElectronicsTransactionCategory.Description, "Electronic stuff");
            Assert.IsTrue(updatedElectronicsTransactionCategory.IsExpenseType);

            //delete description in the category
            updatedElectronicsTransactionCategory = transactionCategoryService.UpdateTransactionCategory(user.Id, createdElectronicsTransactionCategory.Id, new TransactionCategoryUpdateDTOIn
            {
                Name = "Electronics & Stuff",
                IsExpenseType = true
            });
            Assert.IsNotNull(updatedElectronicsTransactionCategory);
            Assert.AreEqual(updatedElectronicsTransactionCategory.Name, "Electronics & Stuff");
            Assert.IsTrue(string.IsNullOrEmpty(updatedElectronicsTransactionCategory.Description));
            Assert.IsTrue(updatedElectronicsTransactionCategory.IsExpenseType);

            updatedElectronicsTransactionCategory = transactionCategoryService.GetTransactionCategory(user.Id, updatedElectronicsTransactionCategory.Id);
            Assert.IsNotNull(updatedElectronicsTransactionCategory);
            Assert.AreEqual(updatedElectronicsTransactionCategory.Name, "Electronics & Stuff");
            Assert.IsTrue(string.IsNullOrEmpty(updatedElectronicsTransactionCategory.Description));
            Assert.IsTrue(updatedElectronicsTransactionCategory.IsExpenseType);

            //change the category from expense to profit and put description back
            updatedElectronicsTransactionCategory = transactionCategoryService.UpdateTransactionCategory(user.Id, createdElectronicsTransactionCategory.Id, new TransactionCategoryUpdateDTOIn
            {
                Name = "Electronics & Stuff",
                Description = "this is now a profit",
                IsExpenseType = false
            });
            Assert.IsNotNull(updatedElectronicsTransactionCategory);
            Assert.AreEqual(updatedElectronicsTransactionCategory.Name, "Electronics & Stuff");
            Assert.AreEqual(updatedElectronicsTransactionCategory.Description, "this is now a profit");
            Assert.IsFalse(updatedElectronicsTransactionCategory.IsExpenseType);

            updatedElectronicsTransactionCategory = transactionCategoryService.GetTransactionCategory(user.Id, updatedElectronicsTransactionCategory.Id);
            Assert.IsNotNull(updatedElectronicsTransactionCategory);
            Assert.AreEqual(updatedElectronicsTransactionCategory.Name, "Electronics & Stuff");
            Assert.AreEqual(updatedElectronicsTransactionCategory.Description, "this is now a profit");
            Assert.IsFalse(updatedElectronicsTransactionCategory.IsExpenseType);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestNotOwnedTransactionCategoryFailingUpdate()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "ambipom22",
                Password = "sbHJBHBBFff333",
                ConfirmPassword = "sbHJBHBBFff333"
            });

            //create the other user's electronics transaction category
            var createdElectronicsTransactionCategory = transactionCategoryService.CreateTransactionCategory(otherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Electronics",
                IsExpenseType = true
            });

            //try to update it with the user, this should be impossbile
            transactionCategoryService.UpdateTransactionCategory(user.Id, createdElectronicsTransactionCategory.Id, new TransactionCategoryUpdateDTOIn
            {
                Name = "Electronicz",
                IsExpenseType = true,
                Description = "this category is not mine"
            });
        }

        [TestMethod]
        public void TestTransactionCategoryDelete()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);
            var transactionSubCategoryService = new TransactionSubCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //create the user's electronics transaction category
            var createdElectronicsTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Electronics",
                IsExpenseType = true
            });

            //and create the sub categories under electronics
            transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                Name = "PC",
                TransactionCategoryId = createdElectronicsTransactionCategory.Id
            });
            transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                Name = "Car",
                TransactionCategoryId = createdElectronicsTransactionCategory.Id
            });

            //create the user's dresses transaction category
            var createdDressesTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Dresses",
                IsExpenseType = true
            });

            //and create the sub categories under dresses
            transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                Name = "Shoes",
                TransactionCategoryId = createdDressesTransactionCategory.Id
            });
            transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                Name = "Jeans",
                TransactionCategoryId = createdDressesTransactionCategory.Id
            });

            //and delete the electronics category
            transactionCategoryService.DeleteTransactionCategory(user.Id, createdElectronicsTransactionCategory.Id);
            //check that the electronics category is now gone
            Assert.IsNull(transactionCategoryService.GetTransactionCategory(user.Id, createdElectronicsTransactionCategory.Id));
            //and the sub categories under electronics are gone too
            Assert.IsFalse(transactionSubCategoryService.GetTransactionSubCategories(user.Id, transactionCategoryId: createdElectronicsTransactionCategory.Id).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestNotOwnedTransactionCategoryFailingDelete()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var transactionCategoryService = new TransactionCategoryService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "merions",
                Password = "Wella111111",
                ConfirmPassword = "Wella111111"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "ambipom22",
                Password = "sbHJBHBBFff333",
                ConfirmPassword = "sbHJBHBBFff333"
            });

            //create the other user's electronics transaction category
            var createdSalaryTransactionCategory = transactionCategoryService.CreateTransactionCategory(otherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Salary",
                IsExpenseType = false
            });

            //try to delete it with user's id, this should be impossible
            transactionCategoryService.DeleteTransactionCategory(user.Id, createdSalaryTransactionCategory.Id);
        }
    }
}
