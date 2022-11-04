using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.CustomValidationRules
{
    public class NoCategoryIfTransfer : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fromAccountId = (Guid?) validationContext.ObjectType.GetProperty("FromAccountId")?.GetValue(validationContext.ObjectInstance, null);

            var toAccountId = (Guid?) validationContext.ObjectType.GetProperty("ToAccountId")?.GetValue(validationContext.ObjectInstance, null);

            if (fromAccountId.HasValue && toAccountId.HasValue && ((Guid?) value).HasValue)
                return new ValidationResult("Tha transfer transactions cannot have a transaction category", new List<string> { "TransactionSubCategoryId" });

            return ValidationResult.Success;
        }
    }
}