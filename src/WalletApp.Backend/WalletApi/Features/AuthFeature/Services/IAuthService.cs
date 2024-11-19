using Authentication.Models;
using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace WalletApi.Features.AuthFeature.Services
{
    public record class RegisterUserParams(User User, string Login, string Password);
    public record class LoginUserParams(User User, string Password);
    public interface IAuthService
    {
        public Task<AccessTokenData> LoginUserAsync(LoginUserParams loginParams, CancellationToken cancellationToken);
        public Task<AccessTokenData> RefreshTokenAsync(AccessTokenData accessTokenData, CancellationToken cancellationToken);
        public Task<IdentityResult> RegisterUserAsync(RegisterUserParams registerParams, CancellationToken cancellationToken);
    }
}