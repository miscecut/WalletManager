using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.Test
{
    public class DbContextGeneration
    {
        private static int _dbNumber = 0;

        public static WalletManagerContext GenerateDb()
        {
            var options = new DbContextOptionsBuilder<WalletManagerContext>()
            .UseInMemoryDatabase(databaseName: "WalletManager_" + _dbNumber++)
            .Options;

            var dbContext = new WalletManagerContext(options);

            var cash = new AccountType()
            {
                Id = Guid.NewGuid(),
                Name = "Cash",
                Description = null
            };
            var bankAccount = new AccountType()
            {
                Id = Guid.NewGuid(),
                Name = "Bank account",
                Description = null
            };

            dbContext.AccountTypes.AddRange(new AccountType[] { cash, bankAccount });
            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
