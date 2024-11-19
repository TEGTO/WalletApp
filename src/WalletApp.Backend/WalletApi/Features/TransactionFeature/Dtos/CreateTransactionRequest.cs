using WalletEntities.Domain.Entities;

namespace WalletApi.Features.TransactionFeature.Dtos
{
    public class CreateTransactionRequest
    {
        public string CardId { get; set; }
        public TransactionType Type { get; set; }
        public decimal Sum { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool Pending { get; set; }
        public string? AuthorizedUserId { get; set; }
    }
}
