using Authentication.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace AuthenticationTests.Token
{
    [TestFixture]
    internal class JwtHandlerTests
    {
        private Mock<JwtSettings> mockJwtSettings;

        [SetUp]
        public void Setup()
        {
            // Mock JwtSettings
            var jwtSettings = new JwtSettings
            {
                Key = "this is super secret key for authentication testing",
                Issuer = "test_issuer",
                Audience = "test_audience",
                ExpiryInMinutes = 30
            };

            mockJwtSettings = new Mock<JwtSettings>();
            mockJwtSettings.SetupGet(settings => settings.Key).Returns(jwtSettings.Key);
            mockJwtSettings.SetupGet(settings => settings.Issuer).Returns(jwtSettings.Issuer);
            mockJwtSettings.SetupGet(settings => settings.Audience).Returns(jwtSettings.Audience);
            mockJwtSettings.SetupGet(settings => settings.ExpiryInMinutes).Returns(jwtSettings.ExpiryInMinutes);
        }

        private JwtHandler CreateJwtHandler()
        {
            return new JwtHandler(
                mockJwtSettings.Object);
        }

        [Test]
        public void CreateToken_ValidData_ValidAccessToken()
        {
            // Arrange
            var user = new IdentityUser
            {
                Email = "test@example.com",
                UserName = "testuser"
            };
            var jwtHandler = CreateJwtHandler();
            // Act
            var accessTokenData = jwtHandler.CreateToken(user, new[] { "Role" });
            // Assert
            Assert.IsNotNull(accessTokenData.AccessToken);
            Assert.IsNotNull(accessTokenData.RefreshToken);
            Assert.IsNotEmpty(accessTokenData.AccessToken);
            Assert.IsNotEmpty(accessTokenData.RefreshToken);
            mockJwtSettings.VerifyAll();
        }
        [Test]
        public void GetPrincipalFromExpiredToken_ValidData_ValidPrincipal()
        {
            // Arrange
            var user = new IdentityUser
            {
                Email = "test@example.com",
                UserName = "testuser"
            };
            var jwtHandler = CreateJwtHandler();
            var accessTokenData = jwtHandler.CreateToken(user, new[] { "Role" });
            // Act
            var principal = jwtHandler.GetPrincipalFromExpiredToken(accessTokenData.AccessToken);
            // Assert
            Assert.IsNotNull(principal);
            Assert.IsTrue(principal.Identity.IsAuthenticated);
        }
        [Test]
        public void GetPrincipalFromExpiredToken_InvalidData_ThrowsException()
        {
            // Arrange
            var jwtHandler = CreateJwtHandler();
            var invalidToken = "invalid_jwt_token";
            // Act & Assert
            Assert.Throws<SecurityTokenMalformedException>(() => jwtHandler.GetPrincipalFromExpiredToken(invalidToken));
        }
    }
}