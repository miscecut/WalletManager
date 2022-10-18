using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("/api/login")]
    public class LoginController : ControllerBase
    {
        #region Properties

        private readonly IUserService _userService;
        private IConfiguration _config;

        #endregion

        #region CTORs

        public LoginController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public IActionResult Login([FromBody] UserLoginDTOIn userLogin)
        {
            var user = _userService.Authenticate(userLogin);

            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(new LoggedUserDTOOut
                {
                    Username = user.Username,
                    Token = token,
                });
            }

            return Unauthorized(new ErrorContainer("password","Credenziali errate"));
        }

        #endregion

        #region Private Methods

        private string GenerateToken(UserDTOOut user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
