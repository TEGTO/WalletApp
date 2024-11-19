using Authentication.Models;
using AuthEntities.Domain.Entities;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services
{
    public interface ITokenService
    {
        public Task<AccessTokenData> CreateNewTokenDataAsync(User user, DateTime refreshTokenExpiryDate, CancellationToken cancellationToken);
        public Task SetRefreshTokenAsync(User user, AccessTokenData accessTokenData, CancellationToken cancellationToken);
        public ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}