using Authentication.Models;
using Authentication.Token;
using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenHandler tokenHandler;
        private readonly UserManager<User> userManager;

        public TokenService(ITokenHandler tokenHandler, UserManager<User> userManager)
        {
            this.tokenHandler = tokenHandler;
            this.userManager = userManager;
        }

        public async Task<AccessTokenData> CreateNewTokenDataAsync(User user, DateTime refreshTokenExpiryDate, CancellationToken cancellationToken)
        {
            var roles = await userManager.GetRolesAsync(user);
            var tokenData = tokenHandler.CreateToken(user, roles);
            tokenData.RefreshTokenExpiryDate = refreshTokenExpiryDate;
            return tokenData;
        }
        public async Task SetRefreshTokenAsync(User user, AccessTokenData accessTokenData, CancellationToken cancellationToken)
        {
            user.RefreshToken = accessTokenData.RefreshToken;
            user.RefreshTokenExpiryTime = accessTokenData.RefreshTokenExpiryDate;
            await userManager.UpdateAsync(user);
        }
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            return tokenHandler.GetPrincipalFromExpiredToken(token);
        }
    }
}
