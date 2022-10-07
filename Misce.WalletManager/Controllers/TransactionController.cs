using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

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
            var userId = GetUserGuid();

            if(userId.HasValue)
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
            var userId = GetUserGuid();

            if (userId.HasValue)
            {
                var transaction = _transactionService.GetTransaction(userId.Value, id);

                if (transaction != null)
                    return Ok(transaction);
                return NotFound("The transaction with ID " + id + " was not found");
            }

            return Unauthorized();
        }

        [HttpPost]
        public IActionResult CreateTransaction(TransactionCreationDTOIn transaction)
        {
            var userId = GetUserGuid();

            if(userId.HasValue)
            {
                try
                {
                    var createdTransaction = _transactionService.CreateTransaction(userId.Value, transaction);

                    return CreatedAtAction(
                        actionName: nameof(GetTransaction),
                        routeValues: new { id = createdTransaction.Id },
                        value: createdTransaction);
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

            return Unauthorized();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteTransaction(Guid id)
        {
            var userId = GetUserGuid();

            if(userId != null)
            {
                try
                {
                    _transactionService.DeleteTransaction(userId.Value, id);
                    return NoContent();
                }
                catch (InvalidDataException e)
                {
                    return NotFound(e.Message);
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
