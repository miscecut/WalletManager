using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.User;
using System.Text.Json;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Route("/api/register")]
    public class RegisterController : ControllerBase
    {
        #region Properties

        private readonly IUserService _userService;

        #endregion

        #region CTORs

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public IActionResult Register(UserSignInDTOIn signInRequest)
        {
            try
            {
                var createdUser = _userService.RegisterUser(signInRequest);
                return Ok(); //TODO: change in Created()
            }
            catch (UsernameNotAvailableException)
            {
                return Conflict("The username " + signInRequest.Username + " is not available");
            }
            catch (IncorrectDataException e)
            {
                return UnprocessableEntity(JsonSerializer.Deserialize<ErrorContainer>(e.Message));
            }
            catch (Exception)
            {
                return Problem("An internal server error occurred");
            }
        }

        #endregion
    }
}
