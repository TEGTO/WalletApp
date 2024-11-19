using Authentication.Models;
using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services.Tests
{
    [TestFixture]
    internal class AuthServiceTests
    {
        private Mock<UserManager<User>> userManagerMock;
        private Mock<ITokenService> tokenServiceMock;
        private Mock<IConfiguration> configurationMock;
        private AuthService authService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();

            userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            tokenServiceMock = new Mock<ITokenService>();

            configurationMock = new Mock<IConfiguration>();

            configurationMock.Setup(c => c[It.Is<string>(s => s == Configuration.AUTH_REFRESH_TOKEN_EXPIRY_IN_DAYS)])
                             .Returns("7");

            authService = new AuthService(userManagerMock.Object, tokenServiceMock.Object, configurationMock.Object);
        }

        [Test]
        public async Task RegisterUserAsync_UserAndPasswordProvided_IdentityResultReturned()
        {
            // Arrange
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var password = "Password123";
            var registerParams = new RegisterUserParams(user, "testuser", password);
            var identityResult = IdentityResult.Success;
            userManagerMock.Setup(x => x.CreateAsync(user, password)).ReturnsAsync(identityResult);
            // Act
            var result = await authService.RegisterUserAsync(registerParams, CancellationToken.None);
            // Assert
            Assert.That(result, Is.EqualTo(identityResult));
        }
        [Test]
        public async Task LoginUserAsync_ValidLoginAndPassword_TokenReturned()
        {
            // Arrange
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var loginParams = new LoginUserParams(user, "Password123");
            var tokenData = new AccessTokenData { AccessToken = "token", RefreshToken = "refreshToken" };
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginParams.Password)).ReturnsAsync(true);
            tokenServiceMock.Setup(x => x.CreateNewTokenDataAsync(user, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokenData);
            userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            // Act
            var result = await authService.LoginUserAsync(loginParams, CancellationToken.None);
            // Assert
            Assert.That(result, Is.EqualTo(tokenData));
        }
        [Test]
        public void LoginUserAsync_InvalidLoginOrPassword_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var user = new User { UserName = "testuser", Email = "testuser@example.com" };
            var loginParams = new LoginUserParams(user, "wrongPassword");
            userManagerMock.Setup(x => x.CheckPasswordAsync(user, loginParams.Password)).ReturnsAsync(false);
            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => authService.LoginUserAsync(loginParams, CancellationToken.None));
        }
        [Test]
        public async Task RefreshTokenAsync_ValidTokenData_ReturnsNewTokenData()
        {
            // Arrange
            var user = new User { Id = "test-user-id", UserName = "testuser", RefreshToken = "valid-refresh-token", RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) };
            var tokenData = new AccessTokenData { AccessToken = "old-access-token", RefreshToken = "valid-refresh-token" };
            var newTokenData = new AccessTokenData { AccessToken = "new-access-token", RefreshToken = "new-refresh-token" };

            userManagerMock.Setup(x => x.FindByNameAsync(user.UserName)).ReturnsAsync(user);
            tokenServiceMock.Setup(x => x.GetPrincipalFromToken(tokenData.AccessToken)).Returns(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.UserName) })));
            tokenServiceMock.Setup(x => x.CreateNewTokenDataAsync(user, It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(newTokenData);
            userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            // Act
            var result = await authService.RefreshTokenAsync(tokenData, CancellationToken.None);
            // Assert
            Assert.That(result, Is.EqualTo(newTokenData));
        }
        [Test]
        public void RefreshTokenAsync_InvalidToken_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var tokenData = new AccessTokenData { AccessToken = "invalid-token", RefreshToken = "invalid-refresh-token" };
            tokenServiceMock.Setup(x => x.GetPrincipalFromToken(tokenData.AccessToken)).Throws<UnauthorizedAccessException>();
            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(() => authService.RefreshTokenAsync(tokenData, CancellationToken.None));
        }
    }
}