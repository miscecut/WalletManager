using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.DTO.DTO.User;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionSubCategoryServiceTest
    {
        [TestMethod]
        public void TestTransactionSubCategoryCreateAndGet()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "wighiiii",
                Password = "ds1bsdbSDDSSD",
                ConfirmPassword = "ds1bsdbSDDSSD"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym transaction category
            var createdVideogamesTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Videogames",
                IsExpenseType = true
            });

            //create the gym transaction category
            var createdFoodTransactionCategory = transactionCategoryService.CreateTransactionCategory(otherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Food",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymProductsTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });
            Assert.IsNotNull(createdGymProductsTransactionSubCategory);
            Assert.AreEqual(createdGymProductsTransactionSubCategory.Name, "Gym products");
            Assert.AreEqual(createdGymProductsTransactionSubCategory.Description, "protein bars, food ecc.");
            Assert.IsNotNull(createdGymProductsTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(createdGymProductsTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(createdGymProductsTransactionSubCategory.TransactionCategory.Name, "Gym");

            //create the gym bill transaction subcategory
            var createdGymBillTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym bill"
            });
            Assert.IsNotNull(createdGymBillTransactionSubCategory);
            Assert.AreEqual(createdGymBillTransactionSubCategory.Name, "Gym bill");
            Assert.IsTrue(string.IsNullOrEmpty(createdGymBillTransactionSubCategory.Description));
            Assert.IsNotNull(createdGymBillTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(createdGymBillTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(createdGymBillTransactionSubCategory.TransactionCategory.Name, "Gym");

            //create the steam transaction subcategory
            var createdSteamTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdVideogamesTransactionCategory.Id,
                Name = "Steam"
            });
            Assert.IsNotNull(createdSteamTransactionSubCategory);
            Assert.AreEqual(createdSteamTransactionSubCategory.Name, "Steam");
            Assert.IsTrue(string.IsNullOrEmpty(createdSteamTransactionSubCategory.Description));
            Assert.IsNotNull(createdSteamTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(createdSteamTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(createdSteamTransactionSubCategory.TransactionCategory.Name, "Videogames");

            //create the ps4 transaction subcategory
            var createdPs5TransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdVideogamesTransactionCategory.Id,
                Name = "PS5"
            });
            Assert.IsNotNull(createdPs5TransactionSubCategory);
            Assert.AreEqual(createdPs5TransactionSubCategory.Name, "PS5");
            Assert.IsTrue(string.IsNullOrEmpty(createdPs5TransactionSubCategory.Description));
            Assert.IsNotNull(createdPs5TransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(createdPs5TransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(createdPs5TransactionSubCategory.TransactionCategory.Name, "Videogames");

            //create the ps4 transaction subcategory
            var createdSwitchTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdVideogamesTransactionCategory.Id,
                Name = "Switch"
            });
            Assert.IsNotNull(createdSwitchTransactionSubCategory);
            Assert.AreEqual(createdSwitchTransactionSubCategory.Name, "Switch");
            Assert.IsTrue(string.IsNullOrEmpty(createdSwitchTransactionSubCategory.Description));
            Assert.IsNotNull(createdSwitchTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(createdSwitchTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(createdSwitchTransactionSubCategory.TransactionCategory.Name, "Videogames");

            //create the food transaction subcategory
            var createdFoodTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(otherUser.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdFoodTransactionCategory.Id,
                Name = "Food"
            });
            Assert.IsNotNull(createdFoodTransactionSubCategory);
            Assert.AreEqual(createdFoodTransactionSubCategory.Name, "Food");
            Assert.IsTrue(string.IsNullOrEmpty(createdFoodTransactionSubCategory.Description));
            Assert.IsNotNull(createdFoodTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(createdFoodTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(createdFoodTransactionSubCategory.TransactionCategory.Name, "Food");

            //get all user's transaction subcategories
            var allTransactionSubCategories = transactionSubCategoryService.GetTransactionSubCategories(user.Id);
            Assert.AreEqual(allTransactionSubCategories.Count(), 5);
            Assert.IsFalse(allTransactionSubCategories.Where(tsc => tsc.Name == "Food").Any());

            //get all user's videogames transaction subcategories
            var videogamesTransactionSubCategories = transactionSubCategoryService.GetTransactionSubCategories(user.Id, transactionCategoryId: createdVideogamesTransactionCategory.Id);
            Assert.AreEqual(videogamesTransactionSubCategories.Count(), 3);
            Assert.IsFalse(videogamesTransactionSubCategories.Where(tsc => tsc.TransactionCategory.Name == "Gym").Any());
        }

        [TestMethod]
        public void TestTransactionSubCategoryCreateAndGetSingle()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "wighiiii",
                Password = "ds1bsdbSDDSSD",
                ConfirmPassword = "ds1bsdbSDDSSD"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym transaction category
            var createdFoodTransactionCategory = transactionCategoryService.CreateTransactionCategory(otherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Food",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymProductsTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });

            //create the gym bill transaction subcategory
            var createdGymBillTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym bill"
            });

            //create the food transaction subcategory
            var createdFoodTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(otherUser.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdFoodTransactionCategory.Id,
                Name = "Food"
            });

            //get the gym bills transaction subcategory
            var gymBillsTransactionSubCategory = transactionSubCategoryService.GetTransactionSubCategory(user.Id, createdGymBillTransactionSubCategory.Id);
            Assert.IsNotNull(gymBillsTransactionSubCategory);
            Assert.AreEqual(gymBillsTransactionSubCategory.Name, "Gym bill");
            Assert.IsTrue(string.IsNullOrEmpty(gymBillsTransactionSubCategory.Description));
            Assert.IsNotNull(gymBillsTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(gymBillsTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(gymBillsTransactionSubCategory.TransactionCategory.Name, "Gym");
            Assert.IsTrue(createdGymBillTransactionSubCategory.TransactionCategory.IsExpenseType);

            var foodTransactionSubCategory = transactionSubCategoryService.GetTransactionSubCategory(user.Id, createdFoodTransactionSubCategory.Id);
            Assert.IsNull(foodTransactionSubCategory);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestUnprovidedNameFailingTransactionSubCategoryCreate()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //try to create the gym products transaction subcategory without the name
            transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Description = "protein bars, food ecc."
            });
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestUnprovidedTransactionCategoryIdFailingTransactionSubCategoryCreate()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the gym transaction category
            transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //try to create the gym products transaction subcategory without the category id
            transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });
        }

        [TestMethod]
        public void TestTransactionSubCategoryUpdate()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });

            //update the gym products subcategory
            var updatedGymProductsTransactionSubCategory = transactionSubCategoryService.UpdateTransactionSubCategory(user.Id, createdGymTransactionSubCategory.Id, new TransactionSubCategoryUpdateDTOIn
            {
                TransactionCategoryId = createdGymTransactionSubCategory.TransactionCategory.Id,
                Name = "Gym stuff",
                Description = "protein bars, food, protein shakes ecc."
            });
            Assert.IsNotNull(updatedGymProductsTransactionSubCategory);
            Assert.AreEqual(updatedGymProductsTransactionSubCategory.Name, "Gym stuff");
            Assert.AreEqual(updatedGymProductsTransactionSubCategory.Description, "protein bars, food, protein shakes ecc.");
            Assert.IsNotNull(updatedGymProductsTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(updatedGymProductsTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(updatedGymProductsTransactionSubCategory.TransactionCategory.Name, "Gym");
            Assert.IsTrue(updatedGymProductsTransactionSubCategory.TransactionCategory.IsExpenseType);

            updatedGymProductsTransactionSubCategory = transactionSubCategoryService.GetTransactionSubCategory(user.Id, updatedGymProductsTransactionSubCategory.Id);
            Assert.IsNotNull(updatedGymProductsTransactionSubCategory);
            Assert.AreEqual(updatedGymProductsTransactionSubCategory.Name, "Gym stuff");
            Assert.AreEqual(updatedGymProductsTransactionSubCategory.Description, "protein bars, food, protein shakes ecc.");
            Assert.IsNotNull(updatedGymProductsTransactionSubCategory.TransactionCategory);
            Assert.IsNotNull(updatedGymProductsTransactionSubCategory.TransactionCategory.Id);
            Assert.AreEqual(updatedGymProductsTransactionSubCategory.TransactionCategory.Name, "Gym");
            Assert.IsTrue(updatedGymProductsTransactionSubCategory.TransactionCategory.IsExpenseType);
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestUnownedTransactionSubCategoryFailingUpdate()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "maurone",
                Password = "sdgsdgsdFF1FFFF",
                ConfirmPassword = "sdgsdgsdFF1FFFF"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(otherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(otherUser.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });

            //try to update the gym products subcategory without providing a name
            transactionSubCategoryService.UpdateTransactionSubCategory(user.Id, createdGymTransactionSubCategory.Id, new TransactionSubCategoryUpdateDTOIn
            {
                TransactionCategoryId = createdGymTransactionSubCategory.TransactionCategory.Id,
                Name = "Gym stuff"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestNoNameTransactionSubCategoryFailingUpdate()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });

            //try to update the gym products subcategory without providing a name
            transactionSubCategoryService.UpdateTransactionSubCategory(user.Id, createdGymTransactionSubCategory.Id, new TransactionSubCategoryUpdateDTOIn
            {
                TransactionCategoryId = createdGymTransactionSubCategory.TransactionCategory.Id
            });
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestNoTransactionCategoryTransactionSubCategoryFailingUpdate()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(user.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(user.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });

            //try to update the gym products subcategory without providing a transaction category
            transactionSubCategoryService.UpdateTransactionSubCategory(user.Id, createdGymTransactionSubCategory.Id, new TransactionSubCategoryUpdateDTOIn
            {
                Name = "Gym products",
                Description = "New description?"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ElementNotFoundException))]
        public void TestUnownedTransactionSubCategoryFailingDelete()
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
                Username = "alcer94",
                Password = "VUFgukfFKF34",
                ConfirmPassword = "VUFgukfFKF34"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "maurone",
                Password = "sdgsdgsdFF1FFFF",
                ConfirmPassword = "sdgsdgsdFF1FFFF"
            });

            //create the gym transaction category
            var createdGymTransactionCategory = transactionCategoryService.CreateTransactionCategory(otherUser.Id, new TransactionCategoryCreationDTOIn
            {
                Name = "Gym",
                IsExpenseType = true
            });

            //create the gym products transaction subcategory
            var createdGymTransactionSubCategory = transactionSubCategoryService.CreateTransactionSubCategory(otherUser.Id, new TransactionSubCategoryCreationDTOIn
            {
                TransactionCategoryId = createdGymTransactionCategory.Id,
                Name = "Gym products",
                Description = "protein bars, food ecc."
            });

            //try to delete an unowned transaction subcategory
            transactionSubCategoryService.DeleteTransactionSubCategory(user.Id, createdGymTransactionSubCategory.Id);
        }
    }
}
