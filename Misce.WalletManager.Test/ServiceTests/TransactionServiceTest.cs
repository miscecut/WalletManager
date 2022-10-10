﻿using Misce.WalletManager.BL.Classes;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.DTO.DTO.Transaction;
using Misce.WalletManager.Model.Data;

namespace Misce.WalletManager.Test.ServiceTests
{
    [TestClass]
    public class TransactionServiceTest
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
            _dbContext = DbContextGeneration.GenerateDb("TEST_TRANSACTION_SERVICE", _saddamId, _misceId, _svetlanaId);
        }

        [TestMethod]
        public void TestGetTransactions()
        {
            //initialize the services
            var transactionService = new TransactionService(_dbContext);

            //saddam has 4 transactions
            var saddamTransactions = transactionService.GetTransactions(_saddamId, 100, 0);
            Assert.AreEqual(saddamTransactions.Count(), 4);
            Assert.AreEqual(saddamTransactions.Where(t => t.Amount == 3.5M).Count(), 2);
            Assert.AreEqual(saddamTransactions.Where(t => t.FromAccount != null && t.FromAccount.Name == "Banco Allah").Count(), 3);
            Assert.AreEqual(saddamTransactions.Where(t => t.ToAccount != null && t.ToAccount.Name == "Banco Allah").Count(), 1);
            Assert.AreEqual(saddamTransactions.Where(t => t.TransactionSubCategory != null && t.TransactionSubCategory.Name == "Food").Count(), 2);

            //saddam has only 3 transactions before the 3rd of february
            var saddamTransactionsOnThe23 = transactionService.GetTransactions(_saddamId, 10, 0, toDateTime: new DateTime(2022, 2, 23, 23, 59, 59));
            Assert.AreEqual(saddamTransactionsOnThe23.Count(), 3);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingCreateTransaction1()
        {
            //initialize the services
            var transactionService = new TransactionService(_dbContext);

            //try to create the transaction under no accounts
            transactionService.CreateTransaction(_misceId, new TransactionCreationDTOIn
            {
                Amount = 50,
                DateTime = DateTime.UtcNow,
                Title = "Paghetta",
                Description = "yeeee soldi"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectDataException))]
        public void TestFailingCreateTransaction2()
        {
            //initialize the services
            var transactionService = new TransactionService(_dbContext);
            var transactionSubCategoryService = new TransactionSubCategoryService(_dbContext);
            var accountService = new AccountService(_dbContext);

            //get the misce's cash account
            var misceCash = accountService.GetAccounts(_misceId).Where(a => a.Name == "Contanti").First();
            Assert.IsNotNull(misceCash);

            //get svetlana's intimo transaction subcategory
            var svetlanaUnderwear = transactionSubCategoryService.GetTransactionSubCategories(_svetlanaId).Where(tsc => tsc.Name == "Intimo").First();
            Assert.IsNotNull(svetlanaUnderwear);

            //try to create the transaction under a not owned transaction subcategory
            transactionService.CreateTransaction(_misceId, new TransactionCreationDTOIn
            {
                Amount = 43.23M,
                FromAccountId = misceCash.Id,
                DateTime = DateTime.UtcNow,
                Title = "Spesa alimentare",
                SubCategoryId = svetlanaUnderwear.Id
            });
        }

        [TestMethod]
        public void TestCreateTransaction()
        {
            //initialize the services
            var transactionService = new TransactionService(_dbContext);
            var accountService = new AccountService(_dbContext);
            var subCategoryService = new TransactionSubCategoryService(_dbContext);

            //get the users' accounts
            var misceAccounts = accountService.GetAccounts(_misceId);
            var saddamAccounts = accountService.GetAccounts(_saddamId);

            //get saddam's transaction subcategories
            var saddamSubCategories = subCategoryService.GetTransactionSubCategories(_saddamId);
            var saddamDrugs = saddamSubCategories.Where(sc => sc.Name == "Drugs").Single();

            //get users' specific accounts
            var misceCash = misceAccounts.Where(a => a.Name == "Contanti").Single();
            var saddamBankAccount = saddamAccounts.Where(a => a.Name == "Banco Allah").Single();

            //GAIN WITH NO SUBCATEGORY

            var misceGainId = transactionService.CreateTransaction(_misceId, new TransactionCreationDTOIn
            {
                Amount = 50,
                ToAccountId = misceCash.Id,
                DateTime = DateTime.UtcNow,
                Title = "Paghetta"
            });

            Assert.IsNotNull(misceGainId);
            Assert.AreEqual(transactionService.GetTransactions(_misceId, 10, 0, toAccountId: misceCash.Id).Count(), 1);

            //GAIN WITH SUBCATEGORY

            var saddamGainId = transactionService.CreateTransaction(_saddamId, new TransactionCreationDTOIn
            {
                Amount = 69.42M,
                SubCategoryId = saddamDrugs.Id,
                ToAccountId = saddamBankAccount.Id,
                DateTime = DateTime.UtcNow,
                Title = "Weed",
                Description = "Got some money baby",
            });

            Assert.IsNotNull(saddamGainId);
            Assert.AreEqual(transactionService.GetTransactions(_saddamId, 10, 0, subCategoryId: saddamDrugs.Id).Count(), 2);

            //PAYMENT WITHOUT SUBCATEGORY AND TITLE

            var miscePayment = transactionService.CreateTransaction(_misceId, new TransactionCreationDTOIn
            {
                Amount = 10,
                FromAccountId = misceCash.Id,
                DateTime = DateTime.UtcNow,
                Description = "Paid some money baby!!!",
            });

            Assert.IsNotNull(miscePayment);
            Assert.AreEqual(transactionService.GetTransactions(_misceId, 10, 0, fromAccountId: misceCash.Id).Count(), 2);
            Assert.AreEqual(miscePayment.Amount, 10);
            Assert.IsTrue(string.IsNullOrEmpty(miscePayment.Title));
            Assert.IsNull(miscePayment.ToAccount);
        }

        [TestMethod]
        public void TestDeleteTransaction()
        {
            //initialize the services
            var transactionService = new TransactionService(_dbContext);

            //get misce's gpu payment transaction
            var misceGPUPayments = transactionService.GetTransactions(_misceId, 10, 0, title: "gt");
            Assert.AreEqual(misceGPUPayments.Count(), 1);
            var misceGPUPayment = misceGPUPayments.First();
            Assert.AreEqual(misceGPUPayment.Title, "GT 610");

            //and delete it
            transactionService.DeleteTransaction(_misceId, misceGPUPayment.Id);

            //verify that the transaction was deleted
            misceGPUPayments = transactionService.GetTransactions(_misceId, 10, 0, title: "gt");
            Assert.IsFalse(misceGPUPayments.Any());
        }
    }
}
