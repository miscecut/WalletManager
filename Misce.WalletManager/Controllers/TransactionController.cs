using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Misce.WalletManager.BL.Classes.ErrorMessages;
using Misce.WalletManager.BL.Classes.Utils;
using Misce.WalletManager.BL.Exceptions;
using Misce.WalletManager.BL.Interfaces;
using Misce.WalletManager.DTO.DTO.Transaction;
using System.Security.Claims;
using System.Text.Json;

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
            Guid? transactionCategoryId = null,
            Guid? transactionSubCategory = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var userId = Utils.GetUserId(HttpContext.User.Identity as ClaimsIdentity);

            if (userId.HasValue)
            {
                var transactions = _transactionService.GetTransactions(userId.Value, limit.GetValueOrDefault(), page.GetValueOrDefault(), title, accountFromId, accountToId, transactionCategoryId, transactionSubCategory, dateFrom, dateTo);

                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase.ToUriComponent()}{request.Path.Value}{request.QueryString}";

                //add next page header & total pages header if limit was reached
                if (transactions.Count() == limit)
                {
                    Response.Headers.Add("X-Next-Page", baseUrl.Replace($"page={page}", $"page={page + 1}"));
                    Response.Headers.Add("X-Total-Pages", _transactionService.GetTransactionsCount(userId.Value, title, accountFromId, accountToId, transactionCategoryId, transactionSubCategory, dateFrom, dateTo).ToString());
                }
                //add previous page header if page is not the first (0)
                if(page > 0)
                    Response.Headers.Add("X-Previous-Page", baseUrl.Replace($"page={page}", $"page={page - 1}"));

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
                return transaction != null ? Ok(transaction) : NotFound();
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
                    return UnprocessableEntity(JsonSerializer.Deserialize<ErrorContainer>(e.Message));
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
                    return Problem();
                }
            }

            return Unauthorized();
        }

        #endregion
    }
}
