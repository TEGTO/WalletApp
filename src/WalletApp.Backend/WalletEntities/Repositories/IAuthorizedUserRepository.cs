using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories
{
    public interface IAuthorizedUserRepository
    {
        public Task<AuthorizedUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken);
        public Task<AuthorizedUser?> AddUserAsync(AuthorizedUser user, CancellationToken cancellationToken);
    }
}