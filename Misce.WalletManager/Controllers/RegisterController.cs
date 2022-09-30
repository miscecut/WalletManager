using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Route("/api/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register(UserLoginDTOIn request)
        {
            var username = request.Username;
            var password = request.Password;

            if (string.IsNullOrEmpty(username))
                return UnprocessableEntity("The username was not provided.");
            if (string.IsNullOrEmpty(password))
                return UnprocessableEntity("The password was not provided.");
            if (password.Length < 8)
                return UnprocessableEntity("The password is too short.");

            if (_userService.IsUsernameAlreadyTaken(username))
                return Conflict("The username " + username + " is not available.");

            var createdUser = _userService.RegisterUser(request);

            return Ok(); //TODO: change in Created()
        }
    }
}
