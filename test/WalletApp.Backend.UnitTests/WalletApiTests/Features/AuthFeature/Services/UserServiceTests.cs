using AuthEntities.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace WalletApi.Features.AuthFeature.Services.Tests
{
    [TestFixture]
    internal class UserServiceTests
    {
        private Mock<UserManager<User>> userManagerMock;
        private Mock<RoleManager<IdentityRole>> roleManagerMock;
        private UserService userService;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var configurationMock = new Mock<IConfiguration>();

            userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object, null, null, null, null);

            userService = new UserService(userManagerMock.Object, configurationMock.Object);
        }

        [Test]
        public async Task GetUserAsync_ValidClaimsPrincipal_UserReturned()
        {
            // Arrange
            var user = new User { Id = "test-user-id", UserName = "testuser" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }));
            userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);
            // Act
            var result = await userService.GetUserAsync(claimsPrincipal, CancellationToken.None);
            // Assert
            Assert.That(result, Is.EqualTo(user));
        }
        [Test]
        public async Task GetUserAsync_InvalidClaimsPrincipal_ReturnsNull()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            // Act
            var result = await userService.GetUserAsync(claimsPrincipal, CancellationToken.None);
            // Assert
            Assert.That(result, Is.Null);
        }
        [Test]
        public async Task GetUserByUserInfoAsync_UserFoundByEmail_UserReturned()
        {
            // Arrange
            var user = new User { Id = "test-user-id", UserName = "testuser", Email = "testuser@example.com" };
            userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            // Act
            var result = await userService.GetUserByLoginAsync(user.Email, CancellationToken.None);
            // Assert
            Assert.That(result, Is.EqualTo(user));
        }
        [Test]
        public async Task GetUserByUserInfoAsync_UserNotFound_ReturnsNull()
        {
            // Arrange
            var info = "non-existent-user";
            userManagerMock.Setup(x => x.FindByEmailAsync(info)).ReturnsAsync((User)null);
            userManagerMock.Setup(x => x.FindByNameAsync(info)).ReturnsAsync((User)null);
            userManagerMock.Setup(x => x.FindByIdAsync(info)).ReturnsAsync((User)null);
            // Act
            var result = await userService.GetUserByLoginAsync(info, CancellationToken.None);
            // Assert
            Assert.That(result, Is.Null);
        }
    }
}