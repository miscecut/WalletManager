using Misce.WalletManager.BL.Exceptions;
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

        public TransactionSubCategoryDTOOut? GetTransactionSubCategory(Guid userId, Guid transactionSubCategoryId)
        {
            var transactionSubCategoryQuery = from transactionSubCategory in _walletManagerContext.TransactionSubCategories
                                              where transactionSubCategory.Category.User.Id == userId
                                              && transactionSubCategory.Id == transactionSubCategoryId
                                              select new TransactionSubCategoryDTOOut
                                              {
                                                  Id = transactionSubCategory.Id,
                                                  Name = transactionSubCategory.Name,
                                                  Description = transactionSubCategory.Description,
                                                  TransactionCategory = new TransactionCategoryDTOOut
                                                  {
                                                      Id = transactionSubCategory.Category.Id,
                                                      Name = transactionSubCategory.Category.Name,
                                                      Description = transactionSubCategory.Category.Description
                                                  }
                                              };

            return transactionSubCategoryQuery.FirstOrDefault();
        }

        public IEnumerable<TransactionSubCategoryDTOOut> GetTransactionSubCategories(Guid userId, Guid? transactionCategoryId = null, string? name = null)
        {
            var transactionSubCategoryQuery = from transactionSubCategory in _walletManagerContext.TransactionSubCategories
                                              where transactionSubCategory.Category.User.Id == userId
                                              select new TransactionSubCategoryDTOOut
                                              {
                                                  Id = transactionSubCategory.Id,
                                                  Name = transactionSubCategory.Name,
                                                  Description = transactionSubCategory.Description,
                                                  TransactionCategory = new TransactionCategoryDTOOut
                                                  {
                                                      Id = transactionSubCategory.Category.Id,
                                                      Name = transactionSubCategory.Category.Name,
                                                      Description = transactionSubCategory.Category.Description
                                                  }
                                              };

            if (transactionCategoryId != null)
                transactionSubCategoryQuery = transactionSubCategoryQuery.Where(transactionSubCategory => transactionSubCategory.TransactionCategory.Id == transactionCategoryId);
            if (name != null)
                transactionSubCategoryQuery = transactionSubCategoryQuery.Where(transactionSubCategory => transactionSubCategory.Name.ToUpper().Contains(name.ToUpper()));

            return transactionSubCategoryQuery.ToList();
        }

        public TransactionSubCategoryDTOOut CreateTransactionSubCategory(Guid userId, TransactionSubCategoryCreationDTOIn transactionSubCategory)
        {
            //transaction category creation data validation
            var validationResults = Utils.Utils.ValidateDTO(transactionSubCategory);
            if (!string.IsNullOrEmpty(validationResults))
                throw new IncorrectDataException(validationResults);

            //check if the transaction category, to put the sub cateogry under, exists
            var transactionCategoryQuery = GetTransactionCategoriesQuery(userId, transactionSubCategory.TransactionCategoryId);

            if (transactionCategoryQuery.Any())
            {
                var category = transactionCategoryQuery.First();

                //create the transaction subcategory
                var transactionSubCategoryToCreate = new TransactionSubCategory
                {
                    Category = category,
                    Name = transactionSubCategory.Name,
                    Description = transactionSubCategory.Description
                };
                _walletManagerContext.TransactionSubCategories.Add(transactionSubCategoryToCreate);

                //commit changes in the db
                _walletManagerContext.SaveChanges();

                //return the created transaction subcategory data
                return new TransactionSubCategoryDTOOut
                {
                    Id = transactionSubCategoryToCreate.Id,
                    Name = transactionSubCategoryToCreate.Name,
                    Description = transactionSubCategoryToCreate.Description,
                    TransactionCategory = new TransactionCategoryDTOOut
                    {
                        Id = transactionSubCategoryToCreate.Category.Id,
                        Name = transactionSubCategoryToCreate.Category.Name,
                        Description = transactionSubCategoryToCreate.Category.Description
                    }
                };
            }
            else
                throw new IncorrectDataException("The transaction category ID " + transactionSubCategory.TransactionCategoryId + " was not found");
        }

        public TransactionSubCategoryDTOOut UpdateTransactionSubCategory(Guid userId, Guid transactionSubCategoryId, TransactionSubCategoryUpdateDTOIn transactionSubCategory)
        {
            //transaction category creation data validation
            var validationResults = Utils.Utils.ValidateDTO(transactionSubCategory);
            if (!string.IsNullOrEmpty(validationResults))
                throw new IncorrectDataException(validationResults);

            // check if the transaction sub category to update exists
            var subCategoryQuery = from sc in _walletManagerContext.TransactionSubCategories
                                   where sc.Id == transactionSubCategoryId
                                   && sc.Category.User.Id == userId
                                   select sc;

            if(subCategoryQuery.Any())
            {
                //check if the transaction category, to put the sub cateogry under, exists
                var categoryQuery = GetTransactionCategoriesQuery(userId, transactionSubCategory.TransactionCategoryId);

                if (categoryQuery.Any())
                {
                    var transactionCategory = categoryQuery.First();

                    var transactionSubCategoryToUpdate = subCategoryQuery.First();
                    transactionSubCategoryToUpdate.Name = transactionSubCategory.Name;
                    transactionSubCategoryToUpdate.Description = transactionSubCategory.Description;
                    transactionSubCategoryToUpdate.Category = transactionCategory;
                    transactionSubCategoryToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                    //commit changes in the db
                    _walletManagerContext.SaveChanges();

                    return new TransactionSubCategoryDTOOut
                    {
                        Id = transactionSubCategoryToUpdate.Id,
                        Name = transactionSubCategoryToUpdate.Name,
                        Description = transactionSubCategoryToUpdate.Description,
                        TransactionCategory = new TransactionCategoryDTOOut
                        {
                            Id = transactionCategory.Id,
                            Name = transactionCategory.Name,
                            Description = transactionCategory.Description
                        }
                    };
                }
                else
                    throw new IncorrectDataException("The transaction category ID " + transactionSubCategory.TransactionCategoryId + " was not found");
            }
            else
                throw new ElementNotFoundException();
        }

        public void DeleteTransactionSubCategory(Guid userId, Guid transactionSubCategoryId)
        {
            // check if the transaction sub category to update exists
            var transactionSubCategoryQuery = from sc in _walletManagerContext.TransactionSubCategories
                                              where sc.Id == transactionSubCategoryId
                                              && sc.Category.User.Id == userId
                                              select sc;

            if(transactionSubCategoryQuery.Any())
            {
                //delete the requested transaction subcategory
                _walletManagerContext.TransactionSubCategories.Remove(transactionSubCategoryQuery.First());

                //commit changes in the db
                _walletManagerContext.SaveChanges();
            }
            else
                throw new ElementNotFoundException();
        }

        #endregion

        #region Private Methods

        private IQueryable<TransactionCategory> GetTransactionCategoriesQuery(Guid userId, Guid categoryId)
        {
            return from transactionCategory in _walletManagerContext.TransactionCategories
                   where transactionCategory.User.Id == userId
                   && transactionCategory.Id == categoryId
                   select transactionCategory;
        }

        #endregion
    }
}
