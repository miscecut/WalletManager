using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionCategory;
using System.Security.Claims;
using System.Text.Json;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/transactioncategories")]
    public class TransactionCategoryController : ControllerBase
    {
        #region Properties

        private readonly ITransactionCategoryService _transactionCategoryService;

        #endregion

        #region CTORs

        public TransactionCategoryController(ITransactionCategoryService transactionCategoryService)
        {
            _transactionCategoryService = transactionCategoryService;
        }

        #endregion

        #region Get Methods

        [HttpGet("{id:guid}")]
        public IActionResult GetTransactionCategory(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var transactionCategory = _transactionCategoryService.GetTransactionCategory(userId.Value, id);
                return transactionCategory != null ? Ok(transactionCategory) :  NotFound();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetTransactionCategories(string? name = null, bool? isExpenseType = null)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var transactionCategories = _transactionCategoryService.GetTransactionCategories(userId.Value, name, isExpenseType);
                return Ok(transactionCategories);
            }

            return Unauthorized();
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public IActionResult CreateTransactionCategory(TransactionCategoryCreationDTOIn transactionCategory)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    if (transactionCategory == null)
                        return BadRequest();

                    var createdTransactionCategory = _transactionCategoryService.CreateTransactionCategory(userId.Value, transactionCategory);

                    return CreatedAtAction(
                        actionName: nameof(GetTransactionCategory),
                        routeValues: new { id = createdTransactionCategory.Id },
                        value: createdTransactionCategory);
                }
                catch(IncorrectDataException e)
                {
                    return UnprocessableEntity(JsonSerializer.Deserialize<ErrorContainer>(e.Message));
                }
                catch(UserNotFoundException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
            }

            return Unauthorized();
        }

        #endregion

        #region Put Methods

        [HttpPut("{id:guid}")]
        public IActionResult UpdateTransactionCategory(Guid id, TransactionCategoryUpdateDTOIn transactionCategory)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    var createdTransactionCategory = _transactionCategoryService.UpdateTransactionCategory(userId.Value, id, transactionCategory);
                    return NoContent();
                }
                catch (IncorrectDataException e)
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
        public IActionResult DeleteTransactionCategory(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if(userId.HasValue)
            {
                try
                {
                    _transactionCategoryService.DeleteTransactionCategory(userId.Value, id);
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
