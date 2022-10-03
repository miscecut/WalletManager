using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using System.Security.Claims;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/transactioncategories")]
    public class TransactionCategoryController : ControllerBase
    {
        private readonly ITransactionCategoryService _transactionCategoryService;

        public TransactionCategoryController(ITransactionCategoryService transactionCategoryService)
        {
            _transactionCategoryService = transactionCategoryService;
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetTransactionCategory(Guid id)
        {
            var userId = GetUserGuid();

            if(userId.HasValue)
            {
                try
                {
                    var transactionCategory = _transactionCategoryService.GetTransactionCategories(userId.Value);

                    if(transactionCategory.Any())
                        return Ok(transactionCategory.First());
                    return NotFound();
                }
                catch(Exception)
                {
                    return Problem("An internal server error occurred");
                }
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetTransactionCategories()
        {
            var userId = GetUserGuid();

            if (userId.HasValue)
            {
                try
                {
                    var transactionCategories = _transactionCategoryService.GetTransactionCategories(userId.Value);
                    return Ok(transactionCategories);
                }
                catch (Exception)
                {
                    return Problem("An internal server error occurred");
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult CreateTransactionCategory(TransactionCategoryCreationDTOIn transactionCategory)
        {
            var userId = GetUserGuid();

            if(userId.HasValue)
            {
                try
                {
                    var createdTransactionCategory = _transactionCategoryService.CreateTransactionCategory(userId.Value, transactionCategory);

                    return CreatedAtAction(
                        actionName: nameof(GetTransactionCategory),
                        routeValues: new { id = createdTransactionCategory.Id },
                        value: createdTransactionCategory);
                }
                catch(InvalidDataException e)
                {
                    return UnprocessableEntity(e);
                }
                catch (Exception)
                {
                    return Problem("An internal server error occurred");
                }
            }

            return Unauthorized();
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateTransactionCategory(Guid id, TransactionCategoryUpdateDTOIn transactionCategory)
        {
            var userId = GetUserGuid();

            if (userId.HasValue)
            {
                try
                {
                    var createdTransactionCategory = _transactionCategoryService.UpdateTransactionCategory(userId.Value, id, transactionCategory);

                    return NoContent();
                }
                catch (InvalidDataException e)
                {
                    return UnprocessableEntity(e);
                }
                catch (Exception)
                {
                    return Problem("An internal server error occurred");
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
