using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;
using System.Security.Claims;
using System.Text.Json;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/transactionsubcategories")]
    public class TransactionSubCategoryController : ControllerBase
    {
        #region Properties

        private readonly ITransactionSubCategoryService _transactionSubCategoryService;

        #endregion

        #region CTORs

        public TransactionSubCategoryController(ITransactionSubCategoryService transactionSubCategoryService)
        {
            _transactionSubCategoryService = transactionSubCategoryService;
        }

        #endregion

        #region Get Methods

        [HttpGet("{id:guid}")]
        public IActionResult GetTransactionSubCategory(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var categorySubCategory = _transactionSubCategoryService.GetTransactionSubCategory(userId.Value, id);
                return categorySubCategory != null ? Ok(categorySubCategory) : NotFound();
            }

            return Unauthorized();
        }

        [HttpGet]
        public IActionResult GetTransactionSubCategories(Guid? transactionCategoryId = null, string? name = null)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var categorySubCategory = _transactionSubCategoryService.GetTransactionSubCategories(userId.Value, transactionCategoryId, name);
                return Ok(categorySubCategory);
            }

            return Unauthorized();
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public IActionResult CreateTransactionSubCategory(TransactionSubCategoryCreationDTOIn transactionSubCategory)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

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
                catch(IncorrectDataException e)
                {
                    return UnprocessableEntity(JsonSerializer.Deserialize<ErrorContainer>(e.Message));
                }
                catch(Exception)
                {
                    return Problem();
                }
            }

            return Unauthorized();
        }

        #endregion

        #region Put Methods

        [HttpPut("{id:guid}")]
        public IActionResult UpdateTransactionSubCategory(Guid id, TransactionSubCategoryUpdateDTOIn transactionSubCategory)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    _transactionSubCategoryService.UpdateTransactionSubCategory(userId.Value, id, transactionSubCategory);
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
        public IActionResult DeleteTransactionSubCategory(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    _transactionSubCategoryService.DeleteTransactionSubCategory(userId.Value, id);
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