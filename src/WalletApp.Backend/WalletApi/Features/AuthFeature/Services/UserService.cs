using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> userManager;
        private readonly string loginProvider;

        public UserService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            loginProvider = configuration[Configuration.LOGIN_PROVIDER] ?? "Login";
        }

        #region IUserService Members

        public async Task<User?> GetUserAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
        {
            var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrEmpty(id) ? null : await userManager.FindByIdAsync(id!);
        }
        public async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByLoginAsync(loginProvider, login);
            user = user == null ? await userManager.FindByEmailAsync(login) : user;
            return user;
        }
        public async Task<IdentityResult?> SetUserLoginAsync(User user, string login, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(login))
                throw new ArgumentException("Provider key cannot be null or empty.", nameof(login));

            return await userManager.AddLoginAsync(user, new UserLoginInfo(loginProvider, login, loginProvider.ToUpper()));
        }

        #endregion
    }
}
