using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using Misce.WalletManager.Model.Data;
using Misce.WalletManager.Model.Models;
using System.Security.Principal;

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

        public TransactionCategoryDTOOut? GetTransactionCategory(Guid userId, Guid transactionCategoryId)
        {
            var categoriesQuery = from category in _walletManagerContext.TransactionCategories
                                  where category.User.Id == userId
                                  && category.Id == transactionCategoryId
                                  select new TransactionCategoryDTOOut
                                  {
                                      Id = category.Id,
                                      Name = category.Name,
                                      Description = category.Description,
                                      IsExpenseType = category.IsExpenseCategory
                                  };

            return categoriesQuery.FirstOrDefault();
        }

        public IEnumerable<TransactionCategoryDTOOut> GetTransactionCategories(Guid userId, string? name = null, bool? isExpenseType = null)
        {
            var categoriesQuery = from category in _walletManagerContext.TransactionCategories
                                  where category.User.Id == userId
                                  select new TransactionCategoryDTOOut
                                  {
                                      Id = category.Id,
                                      Name = category.Name,
                                      Description = category.Description,
                                      IsExpenseType = category.IsExpenseCategory
                                  };

            if (name != null)
                categoriesQuery = categoriesQuery.Where(category => category.Name.ToUpper().Contains(name.ToUpper()));
            if (isExpenseType != null)
                categoriesQuery = categoriesQuery.Where(category => category.IsExpenseType == isExpenseType);

            return categoriesQuery.ToList();
        }

        public TransactionCategoryDTOOut CreateTransactionCategory(Guid userId, TransactionCategoryCreationDTOIn transactionCategory)
        {
            //transaction category creation data validation
            var validationResults = Utils.Utils.ValidateDTO(transactionCategory);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //check if the user exists
            var userQuery = from user in _walletManagerContext.Users
                            where user.Id == userId
                            select user;

            if(userQuery.Any())
            {
                //create the transaction category
                var transactionCategoryToCreate = new TransactionCategory
                {
                    User = userQuery.First(),
                    Name = transactionCategory.Name,
                    Description = transactionCategory.Description,
                    IsExpenseCategory = transactionCategory.IsExpenseType.GetValueOrDefault()
                };
                _walletManagerContext.TransactionCategories.Add(transactionCategoryToCreate);

                //commit changes in the db
                _walletManagerContext.SaveChanges();

                //return the created transaction category data
                var createdTransactionCategory = GetTransactionCategory(userId, transactionCategoryToCreate.Id);
                return createdTransactionCategory == null ? throw new Exception() : createdTransactionCategory;
            }
            else
                throw new UserNotFoundException();
        }

        public TransactionCategoryDTOOut UpdateTransactionCategory(Guid userId, Guid transactionCategoryId, TransactionCategoryUpdateDTOIn transactionCategory)
        {
            //transaction category update data validation
            var validationResults = Utils.Utils.ValidateDTO(transactionCategory);
            if (validationResults.Any())
                throw new IncorrectDataException(Utils.Utils.SerializeErrors(validationResults));

            //check if the transaction category the user wants to update exists
            var transactionCategoryQuery = from tc in _walletManagerContext.TransactionCategories
                                           where tc.Id == transactionCategoryId
                                           && tc.User.Id == userId
                                           select tc;

            if(transactionCategoryQuery.Any())
            {
                //update the transaction category values
                var transactionCategoryToUpdate = transactionCategoryQuery.First();
                transactionCategoryToUpdate.Name = transactionCategory.Name;
                transactionCategoryToUpdate.Description = transactionCategory.Description;
                transactionCategoryToUpdate.IsExpenseCategory = transactionCategory.IsExpenseType;
                transactionCategoryToUpdate.LastModifiedDateTime = DateTime.UtcNow;

                //commit changes in the db
                _walletManagerContext.SaveChanges();

                //return the updated transaction category data
                var updatedTransactionCategory = GetTransactionCategory(userId, transactionCategoryId);
                return updatedTransactionCategory == null ? throw new Exception() : updatedTransactionCategory;
            }
            else
                throw new ElementNotFoundException();
        }

        public void DeleteTransactionCategory(Guid userId, Guid transactionCategoryId)
        {
            //check if the transaction category the user wants to update exists
            var transactionCategoryQuery = from transactionCategory in _walletManagerContext.TransactionCategories
                                           where transactionCategory.Id == transactionCategoryId
                                           && transactionCategory.User.Id == userId
                                           select transactionCategory;

            if (transactionCategoryQuery.Any())
            {
                //delete the transaction subcategories first
                var transactionSubCategoriesQuery = from transactionSubCategory in _walletManagerContext.TransactionSubCategories
                                                    where transactionSubCategory.Category.Id == transactionCategoryId
                                                    select transactionSubCategory;

                //but only if there are any
                if(transactionSubCategoriesQuery.Any())
                {
                    foreach (var transactionSubCategory in transactionSubCategoriesQuery.ToList())
                        _walletManagerContext.TransactionSubCategories.Remove(transactionSubCategory);
                }

                //delete the transaction category requested
                var transactionCategoryToDelete = transactionCategoryQuery.First();
                _walletManagerContext.TransactionCategories.Remove(transactionCategoryToDelete);

                //commit changes in the db
                _walletManagerContext.SaveChanges();
            }
            else
                throw new ElementNotFoundException();
        }

        #endregion
    }
}
