using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class TransactionSubCategoryService : ITransactionSubCategoryService
    {
        #region Properties

        private WalletManagerContext _walletManagerContext;

        #endregion

        #region CTORs

        public TransactionSubCategoryService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        #endregion

        #region Public Methods

        public IEnumerable<TransactionSubCategoryDTOOut> GetTransactionSubCategories(Guid userId)
        {
            var query = from subCategory in _walletManagerContext.TransactionSubCategories
                        where subCategory.Category.User.Id == userId
                        select new TransactionSubCategoryDTOOut
                        {
                            Id = subCategory.Id,
                            Name = subCategory.Name,
                            Description = subCategory.Description,
                            Category = new TransactionCategoryDTOOut
                            {
                                Id = subCategory.Category.Id,
                                Name = subCategory.Category.Name,
                                Description = subCategory.Category.Description
                            }
                        };

            return query.ToList();
        }

        public TransactionSubCategoryDTOOut CreateTransactionSubCategory(Guid userId, TransactionSubCategoryCreationDTOIn subCategory)
        {
            var categoryQuery = GetTransactionCategoriesQuery(userId, subCategory.TransactionCategoryId);

            if (categoryQuery.Any())
            {
                var category = categoryQuery.First();

                var subCategoryToCreate = new TransactionSubCategory
                {
                    Category = category,
                    Name = subCategory.Name,
                    Description = subCategory.Description
                };

                _walletManagerContext.TransactionSubCategories.Add(subCategoryToCreate);
                _walletManagerContext.SaveChanges();

                return new TransactionSubCategoryDTOOut
                {
                    Id = subCategoryToCreate.Id,
                    Name = subCategoryToCreate.Name,
                    Description = subCategoryToCreate.Description,
                    Category = new TransactionCategoryDTOOut
                    {
                        Id = subCategoryToCreate.Category.Id,
                        Name = subCategoryToCreate.Category.Name,
                        Description = subCategoryToCreate.Category.Description
                    }
                };
            }

            throw new InvalidDataException("The provided transaction category id is not valid");
        }

        public TransactionSubCategoryDTOOut? UpdateTransactionSubCategory(Guid userId, Guid transactionSubCategoryId, TransactionSubCategoryUpdateDTOIn transactionSubCategory)
        {
            var subCategoryQuery = from sc in _walletManagerContext.TransactionSubCategories
                                   where sc.Id == transactionSubCategoryId
                                   && sc.Category.User.Id == userId
                                   select sc;

            if(subCategoryQuery.Any())
            {
                var categoryQuery = GetTransactionCategoriesQuery(userId, transactionSubCategory.TransactionCategoryId);

                if(categoryQuery.Any())
                {
                    var transactionCategory = categoryQuery.First();

                    var transactionSubCategoryToUpdate = subCategoryQuery.First();
                    transactionSubCategoryToUpdate.Name = transactionSubCategory.Name;
                    transactionSubCategoryToUpdate.Description = transactionSubCategory.Description;
                    transactionSubCategoryToUpdate.Category = transactionCategory;
                    transactionSubCategoryToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                    _walletManagerContext.SaveChanges();

                    return new TransactionSubCategoryDTOOut
                    {
                        Id = transactionSubCategoryToUpdate.Id,
                        Name = transactionSubCategoryToUpdate.Name,
                        Description = transactionSubCategoryToUpdate.Description,
                        Category = new TransactionCategoryDTOOut
                        {
                            Id = transactionCategory.Id,
                            Name = transactionCategory.Name,
                            Description = transactionCategory.Description
                        }
                    };
                }

                throw new InvalidDataException("The provided transaction category id was not provided or is not valid");
            }

            return null; //transaction sub category not found
        }

        #endregion

        #region Private Methods

        private IQueryable<TransactionCategory> GetTransactionCategoriesQuery(Guid userId, Guid categoryId)
        {
            return from category in _walletManagerContext.TransactionCategories
                   where category.User.Id == userId
                   && category.Id == categoryId
                   select category;
        }

        #endregion
    }
}
