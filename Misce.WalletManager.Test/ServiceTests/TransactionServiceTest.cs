using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.User;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionServiceTest
    {
        [TestMethod]
        public void TestTransactionCreateAndGet()
        {
            //initialize the db context
            var dbContext = DbContextGeneration.GenerateDb();

            //initialize the services
            var userService = new UserService(dbContext);
            var accountService = new AccountService(dbContext);
            var accountTypeService = new AccountTypeService(dbContext);
            var transactionService = new TransactionService(dbContext);

            //create the user
            var user = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "miscecut",
                Password = "GS7fss787f",
                ConfirmPassword = "GS7fss787f"
            });

            //create the other user
            var otherUser = userService.RegisterUser(new UserSignInDTOIn
            {
                Username = "nomeacaso",
                Password = "dnvin!IHUIHUIH2",
                ConfirmPassword = "dnvin!IHUIHUIH2"
            });

            //get the cash account type
            var cashAccountTypeId = accountTypeService.GetAccountTypes().Where(at => at.Name == "Cash").First().Id;

            //create the user's cash
            var createdUserCashAccount = accountService.CreateAccount(user.Id, new AccountCreationDTOIn
            {
                Name = "Cash",
                InitialAmount = 500,
                IsActive = true,
                AccountTypeId = cashAccountTypeId
            });

            var createdUserCoinsAccount = accountService.CreateAccount(user.Id, new AccountCreationDTOIn
            {
                Name = "Coins",
                InitialAmount = 7.77M,
                IsActive = true,
                AccountTypeId = cashAccountTypeId
            });


        }
    }
}
