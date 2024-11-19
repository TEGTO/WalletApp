using WalletEntities.Domain.Entities;

namespace WalletEntities.Domain.Dtos
{
    public class TransactionResponse
    {
        public long Id { get; set; }
        public CardResponse Card { get; set; }
        public TransactionType Type { get; set; }
        public decimal Sum { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool Pending { get; set; }
        public AuthorizedUserResponse? AuthorizedUser { get; set; }
        public Icon TransactionIcon { get; set; }
    }
}
