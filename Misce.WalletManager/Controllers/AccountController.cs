using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using System.Security.Claims;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAccountTypeService _accountTypeService;

        public AccountController(
            IAccountService accountService, 
            IAccountTypeService accountTypeService)
        {
            _accountService = accountService;
            _accountTypeService = accountTypeService;
        }

        [HttpPost]
        public IActionResult CreateAccount(AccountCreationDTOIn request)
        {
            try
            {
                var userGuid = GetUserGuid();

                if (userGuid.HasValue)
                {
                    var createdAccountId = _accountService.CreateAccount(userGuid.Value, request);

                    return CreatedAtAction(
                            actionName: nameof(GetAccount),
                            routeValues: new { id = createdAccountId },
                            value: _accountService.GetAccount(createdAccountId, new Guid()));
                }

                //the user identity is null
                return Unauthorized();
            }
            catch(InvalidDataException e)
            {
                return UnprocessableEntity(e.Message);
            }
            catch (Exception)
            {
                return Problem("An internal server error occurred.");
            }
        }

        [HttpGet()]
        public IActionResult GetAccounts()
        {
            var userGuid = GetUserGuid();

            if (userGuid.HasValue)
            {
                var accounts = _accountService.GetAccounts(userGuid.Value);
                return Ok(accounts);
            }

            //the user identity is null
            return Unauthorized();
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetAccount(Guid id)
        {
            var userGuid = GetUserGuid();

            if (userGuid.HasValue)
            {
                var account = _accountService.GetAccount(id, userGuid.Value);

                if (account != null)
                    return Ok(account);

                return NotFound();
            }

            //the user identity is null
            return Unauthorized();
        }

        private Guid? GetUserGuid()
        {
            //retrieve user's claims (needs login)
            var userIdentity = HttpContext.User.Identity as ClaimsIdentity;

            if(userIdentity != null)
            {
                var guidString = userIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? String.Empty;
                if (string.IsNullOrEmpty(guidString))
                    return null;
                return Guid.Parse(guidString);
            }

            return null;
        }
    }
}
