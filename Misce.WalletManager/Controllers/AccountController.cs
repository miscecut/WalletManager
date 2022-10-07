using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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

        public AccountController(IAccountService accountService, IAccountTypeService accountTypeService)
        {
            _accountService = accountService;
            _accountTypeService = accountTypeService;
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
                return NotFound("The account with ID " + id + " was not found");
            }

            //the user identity is null
            return Unauthorized();
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

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult CreateAccount(AccountCreationDTOIn account)
        {
            try
            {
                var userGuid = GetUserGuid();

                if (userGuid.HasValue)
                {
                    var createdAccount = _accountService.CreateAccount(userGuid.Value, account);

                    return CreatedAtAction(
                            actionName: nameof(GetAccount),
                            routeValues: new { id = createdAccount.Id },
                            value: createdAccount);
                }

                //the user identity is null
                return Unauthorized();
            }
            catch (InvalidDataException e)
            {
                return UnprocessableEntity(e.Message);
            }
            catch (Exception)
            {
                return Problem("An internal server error occurred");
            }
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateAccount(Guid id, AccountUpdateDTOIn account)
        {
            var userGuid = GetUserGuid();

            if (userGuid.HasValue)
            {
                try
                {
                    var createdAccount = _accountService.UpdateAccount(userGuid.Value, id, account);
                    if (createdAccount != null)
                        return NoContent();
                    return NotFound();
                }
                catch(InvalidDataException e)
                {
                    return UnprocessableEntity(e.Message);
                }
                catch(Exception)
                {
                    return Problem();
                }
            }

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
