using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using WalletEntities.Data;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories
{
    public class AuthorizedUserRepository : IAuthorizedUserRepository
    {
        private readonly IDatabaseRepository<WalletDbContext> repository;

        public AuthorizedUserRepository(IDatabaseRepository<WalletDbContext> repository)
        {
            this.repository = repository;
        }

        public async Task<AuthorizedUser?> GetUserByIdAsync(string id, CancellationToken cancellationToken)
        {
            var queryable = (await repository.GetQueryableAsync<AuthorizedUser>(cancellationToken)).AsNoTracking();
            return await queryable.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        public async Task<AuthorizedUser?> AddUserAsync(AuthorizedUser user, CancellationToken cancellationToken)
        {
            return await repository.AddAsync(user, cancellationToken);
        }
    }
}
