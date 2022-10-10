using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.BL.Classes.Utils
{
    public class Utils
    {
        // TODO: inserire qui i limiti delle stringhe e renderli statici e pubblici

        //this method returns a string with all the errors in the provided DTO
        public static string ValidateDTO(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(dto, validationContext, validationResults, true);
            return string.Join(",", validationResults.Select(vr => vr.ErrorMessage));
        }
    }
}
