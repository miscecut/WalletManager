﻿using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.Transaction.CustomValidationRules
{
    public class AtLeastOneAccount : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var fromAccountId = (Guid?)validationContext.ObjectType.GetProperty("FromAccountId")?.GetValue(validationContext.ObjectInstance, null);

            var toAccountId = (Guid?)validationContext.ObjectType.GetProperty("ToAccountId")?.GetValue(validationContext.ObjectInstance, null);

            if (!fromAccountId.HasValue && !toAccountId.HasValue)
                return new ValidationResult("The transaction has to be from or to an account");

            return ValidationResult.Success;
        }
    }
}
