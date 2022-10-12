using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.User;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingSignIn1()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_USER_SERVICE_1", saddamId, misceId, svetlanaId);

            //initialize the services
            var userService = new UserService(dbContext);

            //try to create the new user with a too short password
            var newUser = new UserSignInDTOIn
            {
                Username = "gigidag",
                Password = "Wella1!",
                ConfirmPassword = "Wella1!"
            };
            userService.RegisterUser(newUser);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingSignIn2()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_USER_SERVICE_2", saddamId, misceId, svetlanaId);

            //initialize the services
            var userService = new UserService(dbContext);

            //try to create the new user with a password without a number
            var newUser = new UserSignInDTOIn
            {
                Username = "gigidag",
                Password = "wellaAAAAAAAAAAAAAAA",
                ConfirmPassword = "wellaAAAAAAAAAAAAAAA"
            };
            userService.RegisterUser(newUser);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingSignIn3()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_USER_SERVICE_3", saddamId, misceId, svetlanaId);

            //initialize the services
            var userService = new UserService(dbContext);

            //try to create the new user with a mismatch in the password confirmation
            var newUser = new UserSignInDTOIn
            {
                Username = "gigidag",
                Password = "007Gaben",
                ConfirmPassword = "007Gabenz"
            };
            userService.RegisterUser(newUser);
        }

        [TestMethod]
        [ExpectedException(typeof(UsernameNotAvailableException))]
        public void TestSignIn()
        {
            //initialize the db context and the user ids
            var misceId = Guid.NewGuid();
            var saddamId = Guid.NewGuid();
            var svetlanaId = Guid.NewGuid();
            var dbContext = DbContextGeneration.GenerateDb("TEST_USER_SERVICE_4", saddamId, misceId, svetlanaId);

            //initialize the services
            var userService = new UserService(dbContext);

            //create the new user
            var newUser = new UserSignInDTOIn
            {
                Username = "gigidag",
                Password = "Maritte0!?",
                ConfirmPassword = "Maritte0!?"
            };
            userService.RegisterUser(newUser);

            //verify that the new user can now log in
            Assert.IsNotNull(userService.Authenticate(new UserLoginDTOIn { Username = "gigidag", Password = "Maritte0!?" }));

            //try to create the new user with an already taken username
            newUser = new UserSignInDTOIn
            {
                Username = "gigidag",
                Password = "sdvlisw_esyivHGYU2KF",
                ConfirmPassword = "sdvlisw_esyivHGYU2KF"
            };
            userService.RegisterUser(newUser);
        }
    }
}
