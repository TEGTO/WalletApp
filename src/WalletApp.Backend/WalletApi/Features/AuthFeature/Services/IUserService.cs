using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services
{
    public interface IUserService
    {
        public Task<User?> GetUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken);
        public Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
        public Task<IdentityResult?> SetUserLoginAsync(User user, string login, CancellationToken cancellationToken);
    }
}