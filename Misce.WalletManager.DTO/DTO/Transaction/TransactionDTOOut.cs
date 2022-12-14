using Misce.WalletManager.DTO.DTO.Account;
using Misce.WalletManager.DTO.DTO.TransactionSubCategory;

namespace Misce.WalletManager.DTO.DTO.Transaction
{
    public record TransactionDTOOut
    {
        public Guid Id { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public decimal Amount { get; init; }
        public DateTime DateTime { get; init; }
        public AccountReducedDTOOut? FromAccount { get; init; }
        public AccountReducedDTOOut? ToAccount { get; init; }
        public TransactionSubCategoryDTOOut? TransactionSubCategory { get; init; }
    }
}
