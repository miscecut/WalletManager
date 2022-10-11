using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Transaction;
using System.Security.Claims;

namespace Misce.WalletManager.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/transactions")]
    public class TransactionController : ControllerBase
    {
        #region Properties

        private readonly ITransactionService _transactionService;

        #endregion

        #region CTORs

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        #endregion

        #region Get Methods

        [HttpGet]
        public IActionResult GetTransactions(
            int? limit = 10, 
            int? page = 0, 
            string? title = null,
            Guid? accountFromId = null, 
            Guid? accountToId = null, 
            Guid? categoryId = null,
            Guid? subCategoryId = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var transactions = _transactionService.GetTransactions(userId.Value, limit.GetValueOrDefault(), page.GetValueOrDefault(), title, accountFromId, accountToId, categoryId, subCategoryId, dateFrom, dateTo);
                //Response.Headers.Add("Next page", );
                return Ok(transactions);
            }

            return Unauthorized();
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetTransaction(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var transaction = _transactionService.GetTransaction(userId.Value, id);

                if (transaction != null)
                    return Ok(transaction);
                return NotFound();
            }

            return Unauthorized();
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public IActionResult CreateTransaction(TransactionCreationDTOIn transaction)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                try
                {
                    var createdTransaction = _transactionService.CreateTransaction(userId.Value, transaction);

                    return CreatedAtAction(
                        actionName: nameof(GetTransaction),
                        routeValues: new { id = createdTransaction.Id },
                        value: createdTransaction);
                }
                catch (IncorrectDataException e)
                {
                    return UnprocessableEntity(e.Message);
                }
                catch (Exception)
                {
                    return Problem("An internal server error occurred");
                }
            }

            return Unauthorized();
        }

        #endregion

        #region Put Methods

        [HttpPut("{id:guid}")]
        public IActionResult UpdateTransaction(Guid id, TransactionUpdateDTOIn transaction)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId != null)
            {
                try
                {
                    _transactionService.UpdateTransaction(userId.Value, id, transaction);
                    return NoContent();
                }
                catch (IncorrectDataException e)
                {
                    return UnprocessableEntity(e.Message);
                }
                catch (ElementNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    return Problem("An internal server error occurred");
                }
            }

            return Unauthorized();
        }

        #endregion

        #region Delete Methods

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteTransaction(Guid id)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId != null)
            {
                try
                {
                    _transactionService.DeleteTransaction(userId.Value, id);
                    return NoContent();
                }
                catch (ElementNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception)
                {
                    return Problem("An internal server error occurred");
                }
            }

            return Unauthorized();
        }

        #endregion
    }
}
