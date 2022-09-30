using System.ComponentModel.DataAnnotations;

namespace Misce.WalletManager.DTO.DTO.TransactionCategory
{
    public record TransactionCategoryUpdateDTOIn
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }
}
