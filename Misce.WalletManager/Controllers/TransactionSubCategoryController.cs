using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using System.Security.Claims;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/transactionsubcategories")]
    public class TransactionSubCategoryController : ControllerBase
    {
        private readonly ITransactionSubCategoryService _transactionSubCategoryService;

        public TransactionSubCategoryController(ITransactionSubCategoryService transactionSubCategoryService)
        {
            _transactionSubCategoryService = transactionSubCategoryService;
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetTransactionSubCategory(Guid id)
        {
            var userId = GetUserGuid();

            if(userId.HasValue)
            {
                var categorySubCategory = _transactionSubCategoryService.GetTransactionSubCategories(userId.Value);

                if (categorySubCategory.Any())
                    return Ok(categorySubCategory.First());
                return NotFound();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetTransactionSubCategories()
        {
            var userId = GetUserGuid();

            if (userId.HasValue)
            {
                var categorySubCategory = _transactionSubCategoryService.GetTransactionSubCategories(userId.Value);
                return Ok(categorySubCategory);
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult CreateTransactionSubCategory(TransactionSubCategoryCreationDTOIn transactionSubCategory)
        {
            var userId = GetUserGuid();

            if (userId.HasValue)
            {
                try
                {
                    var createdTransactionSubCategory = _transactionSubCategoryService.CreateTransactionSubCategory(userId.Value, transactionSubCategory);

                    return CreatedAtAction(
                            actionName: nameof(GetTransactionSubCategory),
                            routeValues: new { id = createdTransactionSubCategory.Id },
                            value: createdTransactionSubCategory);
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

        [HttpPut("{id:guid}")]
        public IActionResult UpdateTransactionSubCategory(Guid id, TransactionSubCategoryUpdateDTOIn transactionSubCategory)
        {
            var userId = GetUserGuid();

            if (userId.HasValue)
            {
                try
                {
                    var updateResult = _transactionSubCategoryService.UpdateTransactionSubCategory(userId.Value, id, transactionSubCategory);

                    if(updateResult != null)
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

            if (userIdentity != null)
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