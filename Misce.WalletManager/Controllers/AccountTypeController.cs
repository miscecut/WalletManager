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
        private readonly IAccountTypeService _accountTypeService;

        public AccountTypeController(IAccountTypeService accountTypeService)
        {
            _accountTypeService = accountTypeService;
        }

        [HttpGet]
        public IActionResult GetAccountTypes()
        {
            return Ok(_accountTypeService.GetAccountTypes());
        }
    }
}
