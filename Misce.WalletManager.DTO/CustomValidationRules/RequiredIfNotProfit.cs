using Misce.WalletManager.DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.CustomValidationRules
{
    public class RequiredIfNotProfit : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var transactionType = (TransactionType?)validationContext.ObjectType.GetProperty("TransactionType")?.GetValue(validationContext.ObjectInstance, null);

            if (transactionType.HasValue && transactionType.Value != TransactionType.PROFIT && !((Guid?)value).HasValue)
                return new ValidationResult("Tha expense/transfer transactions must have an account from id", new List<string> { "FromAccountId" });

            return ValidationResult.Success;
        }
    }
}
