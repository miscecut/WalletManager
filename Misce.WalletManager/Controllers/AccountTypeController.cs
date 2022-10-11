using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Interfaces;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/accounttypes")]
    public class AccountTypeController : ControllerBase
    {
        #region Properties

        private readonly IAccountTypeService _accountTypeService;

        #endregion

        #region CTORs

        public AccountTypeController(IAccountTypeService accountTypeService)
        {
            _accountTypeService = accountTypeService;
        }

        #endregion

        #region Get methods

        [HttpGet]
        public IActionResult GetAccountTypes()
        {
            return Ok(_accountTypeService.GetAccountTypes());
        }

        #endregion
    }
}
