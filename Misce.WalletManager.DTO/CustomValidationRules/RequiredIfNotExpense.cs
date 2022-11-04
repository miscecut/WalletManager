using Misce.WalletManager.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.CustomValidationRules
{
    public class RequiredIfNotExpense : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var transactionType = (TransactionType?)validationContext.ObjectType.GetProperty("TransactionType")?.GetValue(validationContext.ObjectInstance, null);

            if(transactionType.HasValue && transactionType.Value != TransactionType.EXPENSE && !((Guid?)value).HasValue)
                return new ValidationResult("Tha profit/transfer transactions must have an account to id", new List<string> { "ToAccountId" });
            
            return ValidationResult.Success;
        }
    }
}
