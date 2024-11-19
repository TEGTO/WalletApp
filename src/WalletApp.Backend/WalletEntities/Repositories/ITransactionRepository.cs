using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories
{
    public interface ITransactionRepository
    {
        public Task<Transaction?> GetTransactionByIdAsync(string userId, long id, CancellationToken cancellationToken);
        public Task<Transaction> AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
        public Task<IEnumerable<Transaction>> GetPaginatedAsync(string userId, PaginationRequest req, CancellationToken cancellationToken);
    }
}