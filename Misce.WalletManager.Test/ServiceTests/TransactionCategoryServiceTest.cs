﻿using Misce.WalletManager.BL.Classes;
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
        public void TestGetTransactioNCategories()
        {
            var transactionCateogryService = new TransactionCategoryService(_dbContext);

            var svetlanaTransactionCategories = transactionCateogryService.GetTransactionCategories(_svetlanaId);

            Assert.AreEqual(svetlanaTransactionCategories.Count(), 3);

            var misceTransactionCategories = transactionCateogryService.GetTransactionCategories(_misceId);

            Assert.AreEqual(misceTransactionCategories.Count(), 2);
            Assert.AreEqual(misceTransactionCategories.Where(tc => tc.Name == "Elettronica").Count(), 1);
        }

        [TestMethod]
        public void TestCreateTransactionCategory()
        {
            var transactionCategoryService = new TransactionCategoryService(_dbContext);

            var saddamBikes = new TransactionCategoryCreationDTOIn
            {
                Name = "Bikes",
                Description = "I ride bikes sometimes"
            };

            var saddamBikesId = transactionCategoryService.CreateTransactionCategory(_saddamId, saddamBikes);

            Assert.IsNotNull(saddamBikesId);

            var createdSaddamBikes = transactionCategoryService.GetTransactionCategories(_saddamId).Where(tc => tc.Id == saddamBikesId).First();

            Assert.IsNotNull(createdSaddamBikes);
            Assert.IsFalse(string.IsNullOrEmpty(createdSaddamBikes.Description));
        }

        [TestMethod]
        public void TestUpdateTransactionCategory()
        {
            var transactionCategoryService = new TransactionCategoryService(_dbContext);

            var misceWhores = transactionCategoryService.GetTransactionCategories(_misceId).Where(tc => tc.Name == "Troie").First();

            Assert.IsTrue(string.IsNullOrEmpty(misceWhores.Description));

            var misceWhoresUpdate = new TransactionCategoryUpdateDTOIn
            {
                Name = misceWhores.Name,
                Description = "Un uomo deve pur guzzare"
            };

            transactionCategoryService.UpdateTransactionCategory(_misceId, misceWhores.Id, misceWhoresUpdate);

            misceWhores = transactionCategoryService.GetTransactionCategories(_misceId).Where(tc => tc.Name == "Troie").First();

            Assert.AreEqual(misceWhores.Description, "Un uomo deve pur guzzare");
        }
    }
}
