using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories
{
    public interface ICardRepository
    {
        public Task<Card?> GetCardByIdAsync(string id, CancellationToken cancellationToken);
        public Task<Card> AddCardAsync(Card card, CancellationToken cancellationToken);
    }
}