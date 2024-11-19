using WalletEntities.Domain.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Dtos
{
    public class TransactionsListResponse
    {
        public decimal CardBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal CardLimit { get; set; }
        public string Points { get; set; }
        public string DueText { get; set; }
        public IEnumerable<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();
    }
}
