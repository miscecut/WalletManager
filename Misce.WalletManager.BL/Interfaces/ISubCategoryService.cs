using Misce.WalletManager.DTO.DTO;

namespace Misce.WalletManager.BL.Interfaces
{
    public interface ISubCategoryService
    {
        public IEnumerable<SubCategoryDTOOut> GetSubCategories(Guid userId);
        public Guid CreateSubCategory(Guid userId, SubCategoryDTOIn subCategory);
    }
}
