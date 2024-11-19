using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using WalletEntities.Data;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly IDatabaseRepository<WalletDbContext> repository;

        public CardRepository(IDatabaseRepository<WalletDbContext> repository)
        {
            this.repository = repository;
        }

        public async Task<Card?> GetCardByIdAsync(string id, CancellationToken cancellationToken)
        {
            var queryable = (await repository.GetQueryableAsync<Card>(cancellationToken)).AsNoTracking();
            return await queryable.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task<Card> AddCardAsync(Card card, CancellationToken cancellationToken)
        {
            return await repository.AddAsync(card, cancellationToken);
        }
    }
}
