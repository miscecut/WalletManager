using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;

namespace Misce.WalletManager.BL.Classes
{
    public class SubCategoryService : ISubCategoryService
    {
        private WalletManagerContext _walletManagerContext;

        public SubCategoryService(WalletManagerContext walletManagerContext)
        {
            _walletManagerContext = walletManagerContext;
        }

        public IEnumerable<SubCategoryDTOOut> GetSubCategories(Guid userId)
        {
            var query = from subCategory in _walletManagerContext.SubCategories
                        where subCategory.Category.User.Id == userId
                        select new SubCategoryDTOOut
                        {
                            Id = subCategory.Id,
                            Name = subCategory.Name,
                            Description = subCategory.Description,
                            Category = subCategory.Category.Name
                        };

            return query.ToList();
        }

        public Guid CreateSubCategory(Guid userId, SubCategoryDTOIn subCategory)
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

            throw new InvalidDataException("The provided category id is not valid!");
        }
    }
}
