using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.User;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestTooShortPasswordFailingSignUp()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

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
        public void TestPasswordWithoutNumbersFailingSignUp()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

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
        public void TestPasswordUnconfirmedFailingSignUp()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

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
        public void TestUsernameAlreadyInUseFailingSignUp()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

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

            //try to create the new user with an already taken username
            newUser = new UserSignInDTOIn
            {
                Username = "gigidag",
                Password = "sdvlisw_esyivHGYU2KF",
                ConfirmPassword = "sdvlisw_esyivHGYU2KF"
            };
            userService.RegisterUser(newUser);
        }

        [TestMethod]
        public void TestCorrectSignUpAndLogin()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);

            //create the new user
            var newUser = new UserSignInDTOIn
            {
                Username = "miscecut",
                Password = "Maritte0!?2",
                ConfirmPassword = "Maritte0!?2"
            };
            userService.RegisterUser(newUser);

            //verify that the new user can now log in
            Assert.IsNotNull(userService.Authenticate(new UserLoginDTOIn { Username = "miscecut", Password = "Maritte0!?2" }));
        }

        [TestMethod]
        public void TestWrongCredentialsFailingLogin()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);

            //create the new user
            var newUser = new UserSignInDTOIn
            {
                Username = "miscecut",
                Password = "Maritte0!?2",
                ConfirmPassword = "Maritte0!?2"
            };
            userService.RegisterUser(newUser);

            //verify that the new user can now log in
            Assert.IsNull(userService.Authenticate(new UserLoginDTOIn { Username = "miscecut", Password = "aaaaAAA222!?" }));
        }

        [TestMethod]
        public void TestNonExistingUserFailingLogin()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);

            //create the new user
            var newUser = new UserSignInDTOIn
            {
                Username = "miscecut",
                Password = "Maritte0!?2",
                ConfirmPassword = "Maritte0!?2"
            };
            userService.RegisterUser(newUser);

            //verify that the new user can now log in
            Assert.IsNull(userService.Authenticate(new UserLoginDTOIn { Username = "pipppppo", Password = "aaaaAAA222!?" }));
        }
    }
}
