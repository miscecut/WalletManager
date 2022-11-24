namespace Misce.WalletManager.DTO.DTO.Account
{
    public record AccountWithAmountHistoryDTOOut : AccountDTOOut
    {
        public AccountAmountHistory[] AccountAmountHistory { get; init; } = new AccountAmountHistory[0];
    }

    public record AccountAmountHistory
    {
        public string AtDate { get; init; }
        public decimal Amount { get; init; }
    }
}
