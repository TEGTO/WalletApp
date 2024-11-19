namespace WalletApi.Features.TransactionsListFeature.Dtos
{
    public class BalanceResponse
    {
        public decimal CardBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal CardLimit { get; set; }
    }
}
