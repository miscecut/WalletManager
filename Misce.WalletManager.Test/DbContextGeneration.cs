using Microsoft.EntityFrameworkCore;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.Test
{
    public class DbContextGeneration
    {
        public static WalletManagerContext GenerateDb(string dbName, Guid user1Id, Guid user2Id, Guid user3Id)
        {
            var options = new DbContextOptionsBuilder<WalletManagerContext>()
            .UseInMemoryDatabase(databaseName: "WalletManager_" + dbName)
            .Options;

            var dbContext = new WalletManagerContext(options);

            //USERS

            var saddam69 = new User()
            {
                Id = user1Id,
                Username = "saddam69",
                Password = "xxx",
                Salt = "xxx"
            };
            var miscecut = new User()
            {
                Id = user2Id,
                Username = "miscecut",
                Password = "yyy",
                Salt = "yyy"
            };
            var svetlanal = new User()
            {
                Id = user3Id,
                Username = "svetlanal",
                Password = "zzz",
                Salt = "zzz"
            };

            dbContext.Users.AddRange(new User[] { saddam69, miscecut, svetlanal });

            //ACCOUNT TYPES

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

            //ACCOUNTS

            var saddamBankAccount = new Account()
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                AccountType = bankAccount,
                Name = "Banco Allah",
                Description = "I keep money here in the name of Allah",
                InitialAmount = 420,
                IsActive = true
            };
            var miscecutCash = new Account()
            {
                Id = Guid.NewGuid(),
                User = miscecut,
                AccountType = cash,
                Name = "Contanti",
                InitialAmount = 69,
                IsActive = true
            };
            var miscecutBankAccount = new Account()
            {
                Id = Guid.NewGuid(),
                User = miscecut,
                AccountType = bankAccount,
                Name = "Banco BPM",
                InitialAmount = 0,
                IsActive = false
            };
            var svetlanalCash = new Account()
            {
                Id = Guid.NewGuid(),
                User = svetlanal,
                AccountType = cash,
                Name = "Soldi",
                InitialAmount = 90,
                IsActive = true
            };
            var svetlanalBankAccount = new Account()
            {
                Id = Guid.NewGuid(),
                User = svetlanal,
                AccountType = bankAccount,
                Name = "Sex Bank",
                InitialAmount = 2,
                IsActive = true
            };

            dbContext.Accounts.AddRange(new Account[] { saddamBankAccount, miscecutCash, miscecutBankAccount, svetlanalCash, svetlanalBankAccount });

            //TRANSACTION CATEGORIES

            var saddamBombing = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                Name = "Bombs"
            };
            var saddamFood = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                Name = "Food",
                Description = "I need to eat!"
            };
            var saddamDrugs = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                Name = "Drugs"
            };
            var misceHoes = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = miscecut,
                Name = "Troie"
            };
            var misceElectronics = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = miscecut,
                Name = "Elettronica"
            };
            var svetlanaDresses = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = svetlanal,
                Name = "Vestiti"
            };
            var svetlanaDrugs = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = svetlanal,
                Name = "Droga"
            };
            var svetlanaOnlyfans = new TransactionCategory
            {
                Id = Guid.NewGuid(),
                User = svetlanal,
                Name = "Onlyfans"
            };

            dbContext.Categories.AddRange(new TransactionCategory[] { saddamBombing, saddamFood, misceHoes, misceElectronics, svetlanaDresses, svetlanaDrugs, svetlanaOnlyfans });

            //TRANSACTION SUBCATEGORIES

            var saddamRifles = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = saddamBombing,
                Name = "Rifles"
            };
            var saddamExplosives = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = saddamBombing,
                Name = "Explosives",
                Description = "they go \"BOOOOM\""
            };
            var saddamFoodSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = saddamFood,
                Name = "Food"
            };
            var saddamDrugsSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = saddamDrugs,
                Name = "Drugs"
            };
            var misceElectronicsSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = misceElectronics,
                Name = "Elettronica"
            };
            var misceNigerianHoes = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = misceHoes,
                Name = "Troie nigeriane"
            };
            var svetlanaShoesSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = svetlanaDresses,
                Name = "Scarpe"
            };
            var svetlanaUnderwearSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = svetlanaDresses,
                Name = "Intimo"
            };
            var svetlanaHatsSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = svetlanaDresses,
                Name = "Cappelli"
            };
            var svetlanaOnlyFansSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = svetlanaOnlyfans,
                Name = "Onlyfans"
            };
            var svetlanaDrugsSub = new TransactionSubCategory
            {
                Id = Guid.NewGuid(),
                Category = svetlanaOnlyfans,
                Name = "Droga"
            };

            dbContext.SubCategories.AddRange(new TransactionSubCategory[] { saddamRifles, saddamExplosives, saddamFoodSub, misceElectronicsSub, misceNigerianHoes, svetlanaDrugsSub, svetlanaHatsSub, svetlanaShoesSub, svetlanaUnderwearSub, svetlanaOnlyFansSub });

            //TRANSACTIONS

            var saddamC4 = new Transaction()
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                FromAccount = saddamBankAccount,
                ToAccount = null,
                Amount = 150,
                DateTime = new DateTime(2022, 2, 23, 20, 0, 23), //20:00:23 23/02/2022
                SubCategory = saddamExplosives,
                Title = "C4 for Twin Towers"
            };
            var saddamKebab = new Transaction()
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                FromAccount = saddamBankAccount,
                ToAccount = null,
                Amount = 3.5M,
                DateTime = new DateTime(2022, 2, 23, 21, 2, 45), //21:02:45 23/02/2022
                SubCategory = saddamFoodSub,
                Title = "Kebab"
            };
            var saddamKebab2 = new Transaction()
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                FromAccount = saddamBankAccount,
                ToAccount = null,
                Amount = 3.5M,
                DateTime = new DateTime(2022, 2, 23, 21, 15, 45), //21:15:45 23/02/2022
                SubCategory = saddamFoodSub,
                Title = "Another kebab"
            };
            var saddamWeedGain = new Transaction()
            {
                Id = Guid.NewGuid(),
                User = saddam69,
                FromAccount = null,
                ToAccount = saddamBankAccount,
                Amount = 50,
                DateTime = new DateTime(2022, 2, 24, 15, 34, 49), //15:34:49 24/02/2022
                SubCategory = saddamDrugsSub,
                Title = "Drugs"
            };
            var misceGPU = new Transaction()
            {
                Id = Guid.NewGuid(),
                User = miscecut,
                FromAccount = miscecutCash,
                ToAccount = null,
                Amount = 59.99M,
                DateTime = new DateTime(2022, 3, 12, 11, 0, 1), //11:00:01 12/03/2022
                SubCategory = misceElectronicsSub,
                Title = "GT 610"
            };

            dbContext.Transactions.AddRange(new Transaction[] { saddamC4, saddamKebab, saddamKebab2, saddamWeedGain, misceGPU });

            dbContext.SaveChanges();

            return dbContext;
        }
    }
}
