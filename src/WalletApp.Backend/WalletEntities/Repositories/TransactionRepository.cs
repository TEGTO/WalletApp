using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using WalletEntities.Data;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDatabaseRepository<WalletDbContext> repository;

        public TransactionRepository(IDatabaseRepository<WalletDbContext> repository)
        {
            this.repository = repository;
        }

        #region ITransactionRepository Members

        public async Task<Transaction?> GetTransactionByIdAsync(string userId, long id, CancellationToken cancellationToken)
        {
            var queryable = (await GetQueryableTransactionsAsync(cancellationToken)).AsNoTracking().AsSplitQuery();
            return await queryable.FirstOrDefaultAsync(x => x.Card.UserId == userId && x.Id == id, cancellationToken);
        }
        public async Task<Transaction> AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            return await repository.AddAsync(transaction, cancellationToken);
        }
        public async Task<IEnumerable<Transaction>> GetPaginatedAsync(string userId, PaginationRequest req, CancellationToken cancellationToken)
        {
            var queryable = await GetQueryableTransactionsAsync(cancellationToken);
            List<Transaction> paginatedItems = new List<Transaction>();

            paginatedItems.AddRange(queryable
                  .Where(x => x.Card.UserId == userId)
                  .OrderByDescending(x => x.Date)
                  .Skip((req.PageNumber - 1) * req.PageSize)
                  .Take(req.PageSize)
                  .AsSplitQuery()
                  .AsNoTracking());

            return paginatedItems;
        }

        #endregion

        #region Private Helpers

        private async Task<IQueryable<Transaction>> GetQueryableTransactionsAsync(CancellationToken cancellationToken)
        {
            var queryable = await repository.GetQueryableAsync<Transaction>(cancellationToken);
            return queryable.Include(x => x.AuthorizedUser).Include(x => x.Card).AsNoTracking();
        }

        #endregion
    }
}
