using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class TransactionCategoryService : ITransactionCategoryService
    {
        #region Properties

        private WalletManagerContext _walletManagerContext;

        #endregion

        #region CTORs

        public TransactionCategoryService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        #endregion

        #region Public Methods

        public IEnumerable<TransactionCategoryDTOOut> GetTransactionCategories(Guid userId)
        {
            var categoriesQuery = from category in _walletManagerContext.TransactionCategories
                                  where category.User.Id == userId
                                  select new TransactionCategoryDTOOut
                                  {
                                      Id = category.Id,
                                      Name = category.Name,
                                      Description = category.Description
                                  };

            return categoriesQuery.ToList();
        }

        public TransactionCategoryDTOOut CreateTransactionCategory(Guid userId, TransactionCategoryCreationDTOIn transactionCategory)
        {
            var userQuery = from user in _walletManagerContext.Users
                            where user.Id == userId
                            select user;

            if(userQuery.Any())
            {
                var transactionCategoryToCreate = new TransactionCategory
                {
                    User = userQuery.First(),
                    Name = transactionCategory.Name,
                    Description = transactionCategory.Description
                };

                _walletManagerContext.TransactionCategories.Add(transactionCategoryToCreate);
                _walletManagerContext.SaveChanges();

                return new TransactionCategoryDTOOut
                {
                    Id = transactionCategoryToCreate.Id,
                    Name = transactionCategoryToCreate.Name,
                    Description = transactionCategoryToCreate.Description
                };
            }

            throw new InvalidDataException("The user was not found");
        }

        public TransactionCategoryDTOOut? UpdateTransactionCategory(Guid userId, Guid transactionCategoryId, TransactionCategoryUpdateDTOIn transactionCategory)
        {
            var transactionCategoryQuery = from tc in _walletManagerContext.TransactionCategories
                                           where tc.Id == transactionCategoryId
                                           && tc.User.Id == userId
                                           select tc;

            if(transactionCategoryQuery.Any())
            {
                var transactionCategoryToUpdate = transactionCategoryQuery.First();
                transactionCategoryToUpdate.Name = transactionCategory.Name;
                transactionCategoryToUpdate.Description = transactionCategory.Description;
                transactionCategoryToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                _walletManagerContext.SaveChanges();

                return new TransactionCategoryDTOOut
                {
                    Id = transactionCategoryToUpdate.Id,
                    Name = transactionCategoryToUpdate.Name,
                    Description= transactionCategoryToUpdate.Description
                };
            }

            return null;
        }

        #endregion
    }
}
