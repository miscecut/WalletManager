using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.Enums;
using System.Security.Claims;
using System.Text.Json;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/accounts")]
    public class AccountController : ControllerBase
    {
        #region Properties

        private readonly IAccountService _accountService;
        private readonly IAccountTypeService _accountTypeService;

        #endregion

        #region CTORs

        public AccountController(IAccountService accountService, IAccountTypeService accountTypeService)
        {
            _accountService = accountService;
            _accountTypeService = accountTypeService;
        }

        #endregion

        #region Get Methods

        [HttpGet("{id:guid}")]
        public IActionResult GetAccount(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var account = _accountService.GetAccount(userId.Value, id);
                return account != null ? Ok(account) : NotFound();
            }

            return Unauthorized();
        }

        [HttpGet()]
        public IActionResult GetAccounts(Guid? accountTypeId = null, bool? active = null)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var accounts = _accountService.GetAccounts(userId.Value, accountTypeId, active);
                return Ok(accounts);
            }

            return Unauthorized();
        }

        [HttpGet("history")]
        public IActionResult GetAccountAmountHistory(Guid? accountTypeId = null, bool? active = null, GroupByPeriod period = GroupByPeriod.MONTH)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var accountsHistory = _accountService.GetAccountsWithAmountHistory(userId.Value, active, period);
                return Ok(accountsHistory);
            }

            return Unauthorized();
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public IActionResult CreateAccount(AccountCreationDTOIn account)
        {
            try
            {
                var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

                if (userId.HasValue)
                {
                    var createdAccount = _accountService.CreateAccount(userId.Value, account);

                    return CreatedAtAction(
                            actionName: nameof(GetAccount),
                            routeValues: new { id = createdAccount.Id },
                            value: createdAccount);
                }

                return Unauthorized();
            }
            catch (IncorrectDataException e)
            {
                return UnprocessableEntity(JsonSerializer.Deserialize<ErrorContainer>(e.Message));
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        #endregion

        #region Put Methods

        [HttpPut("{id:guid}")]
        public IActionResult UpdateAccount(Guid id, AccountUpdateDTOIn account)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    var createdAccount = _accountService.UpdateAccount(userId.Value, id, account);
                    return NoContent();
                }
                catch(IncorrectDataException e)
                {
                    return UnprocessableEntity(JsonSerializer.Deserialize<ErrorContainer>(e.Message));
                }
                catch (ElementNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    return Problem();
                }
            }

            return Unauthorized();
        }

        #endregion

        #region Delete Methods

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteAccount(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    _accountService.DeleteAccount(userId.Value, id);
                    return NoContent();
                }
                catch (ElementNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    return Problem();
                }
            }

            return Unauthorized();
        }

        #endregion
    }
}
