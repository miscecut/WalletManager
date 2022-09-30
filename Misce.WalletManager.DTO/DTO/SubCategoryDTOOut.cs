namespace Misce.WalletManager.DTO.DTO
{
    public record SubCategoryDTOOut
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string Category { get; init; } = null!;
    }
}
