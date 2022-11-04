using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.CustomValidationRules
{
    public class Currency : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is not decimal)
                return new ValidationResult("The transaction amount must be a number");

            var decimalNumber100 = ((decimal) value) * 100;

            if (decimalNumber100 != (decimal) Math.Floor(decimalNumber100))
                return new ValidationResult("The transaction amount must have not more that two decimal numbers", new List<string> { "Amount" });

            return ValidationResult.Success;
        }
    }
}
