namespace Misce.WalletManager.DTO.DTO
{
    public record SubCategoryDTOIn
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public Guid CategoryId { get; init; }
    }
}
