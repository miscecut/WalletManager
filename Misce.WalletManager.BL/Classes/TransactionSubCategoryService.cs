using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class TransactionSubCategoryService : ISubCategoryService
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

        public IEnumerable<TransactionSubCategoryDTOOut> GetSubCategories(Guid userId)
        {
            var query = from subCategory in _walletManagerContext.SubCategories
                        where subCategory.Category.User.Id == userId
                        select new TransactionSubCategoryDTOOut
                        {
                            Id = subCategory.Id,
                            Name = subCategory.Name,
                            Description = subCategory.Description,
                            Category = subCategory.Category.Name
                        };

            return query.ToList();
        }

        public Guid CreateSubCategory(Guid userId, TransactionSubCategoryCreationDTOIn subCategory)
        {
            var categoryQuery = from category in _walletManagerContext.Categories
                                where category.User.Id == userId
                                && subCategory.CategoryId == category.Id
                                select category;

            if(categoryQuery.Any())
            {
                var category = categoryQuery.First();

                var subCategoryToCreate = new TransactionSubCategory
                {
                    Category = category,
                    Name = subCategory.Name,
                    Description = subCategory.Description
                };

                _walletManagerContext.SubCategories.Add(subCategoryToCreate);
                _walletManagerContext.SaveChanges();

                return subCategoryToCreate.Id;
            }

            throw new InvalidDataException("The provided transaction category id is not valid");
        }



        #endregion
    }
}
