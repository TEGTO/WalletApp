using Authentication.Models;
using Authentication.Token;
using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services.Tests
{
    [TestFixture]
    internal class TokenServiceTests
    {
        private Mock<ITokenHandler> mockTokenHandler;
        private Mock<UserManager<User>> mockUserManager;
        private TokenService tokenService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();

            mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            mockTokenHandler = new Mock<ITokenHandler>();
            tokenService = new TokenService(mockTokenHandler.Object, mockUserManager.Object);
        }

        [Test]
        public async Task CreateNewTokenDataAsync_ValidUserAndExpiryDate_ReturnsAccessTokenData()
        {
            // Arrange
            var user = new User { UserName = "testuser" };
            var roles = new List<string> { "Admin", "User" };
            var refreshTokenExpiryDate = DateTime.UtcNow.AddDays(7);

            var expectedTokenData = new AccessTokenData
            {
                AccessToken = "test_access_token",
                RefreshToken = "test_refresh_token",
                RefreshTokenExpiryDate = refreshTokenExpiryDate
            };
            mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);
            mockTokenHandler.Setup(m => m.CreateToken(user, roles)).Returns(expectedTokenData);
            // Act
            var result = await tokenService.CreateNewTokenDataAsync(user, refreshTokenExpiryDate, CancellationToken.None);
            // Assert
            Assert.That(result, Is.EqualTo(expectedTokenData));
        }
        [Test]
        public async Task SetRefreshTokenAsync_ValidUserAndTokenData_UpdatesUserRefreshToken()
        {
            // Arrange
            var user = new User { UserName = "testuser" };
            var accessTokenData = new AccessTokenData
            {
                RefreshToken = "test_refresh_token",
                RefreshTokenExpiryDate = DateTime.UtcNow.AddDays(7)
            };
            mockUserManager.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
            // Act
            await tokenService.SetRefreshTokenAsync(user, accessTokenData, CancellationToken.None);
            // Assert
            mockUserManager.Verify(m => m.UpdateAsync(It.Is<User>(u => u.RefreshToken == accessTokenData.RefreshToken && u.RefreshTokenExpiryTime == accessTokenData.RefreshTokenExpiryDate)), Times.Once);
        }
        [Test]
        public void GetPrincipalFromExpiredToken_ValidToken_ReturnsClaimsPrincipal()
        {
            // Arrange
            var token = "expired_token";
            var expectedPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, "testuser") }));
            mockTokenHandler.Setup(m => m.GetPrincipalFromExpiredToken(token)).Returns(expectedPrincipal);
            // Act
            var result = tokenService.GetPrincipalFromToken(token);
            // Assert
            Assert.That(result, Is.EqualTo(expectedPrincipal));
        }
    }
}