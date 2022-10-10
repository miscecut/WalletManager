using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

        //this method extracts the user id from the claims in the http context
        public static Guid? GetUserId(ClaimsIdentity? claimsIdentity)
        {
            if (claimsIdentity != null)
            {
                var guidString = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? String.Empty;
                if (string.IsNullOrEmpty(guidString))
                    return null;
                return Guid.Parse(guidString);
            }

            return null;
        }
    }
}
