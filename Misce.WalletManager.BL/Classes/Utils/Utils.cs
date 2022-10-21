using Misce.WalletManager.BL.Classes.ErrorMessages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace Misce.WalletManager.BL.Classes.Utils
{
    public class Utils
    {
        public static IEnumerable<ValidationResult> ValidateDTO(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(dto, validationContext, validationResults, true);
            return validationResults;
        }

        public static string SerializeSingleError(string field, string errorMessage)
        {
            return JsonSerializer.Serialize(new ErrorContainer(field, errorMessage));
        }

        //this method returns a json with all the errors in the provided validation result list
        public static string SerializeErrors(IEnumerable<ValidationResult> validationResults)
        {
            var errorsList = new List<Error>();

            foreach (var validationResult in validationResults)
                errorsList.Add(new Error(string.Join("; ", validationResult.MemberNames), validationResult.ErrorMessage == null ? "" : validationResult.ErrorMessage));

            var errorContainer = new ErrorContainer(errorsList);

            return JsonSerializer.Serialize(errorContainer);
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
