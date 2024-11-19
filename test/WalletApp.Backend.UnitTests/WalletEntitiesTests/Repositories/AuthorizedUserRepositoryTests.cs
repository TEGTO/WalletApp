using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using WalletEntities.Data;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories.Tests
{
    [TestFixture]
    internal class AuthorizedUserRepositoryTests
    {
        private Mock<IDatabaseRepository<WalletDbContext>> repositoryMock;
        private CancellationToken cancellationToken;
        private AuthorizedUserRepository repository;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IDatabaseRepository<WalletDbContext>>();
            repository = new AuthorizedUserRepository(repositoryMock.Object);
            cancellationToken = new CancellationToken();
        }

        private static Mock<DbSet<T>> GetDbSetMock<T>(List<T> data) where T : class
        {
            return data.AsQueryable().BuildMockDbSet();
        }

        [Test]
        public async Task GetUserByIdAsync_ValidId_ReturnsAuthorizedUser()
        {
            // Arrange
            var user = new AuthorizedUser
            {
                Id = "user123",
                Name = "TestUser"
            };
            var users = new List<AuthorizedUser> { user };
            var dbSetMock = GetDbSetMock(users);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<AuthorizedUser>(cancellationToken))
               .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await repository.GetUserByIdAsync("user123", cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            repositoryMock.Verify(repo => repo.GetQueryableAsync<AuthorizedUser>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetUserByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var users = new List<AuthorizedUser>();
            var dbSetMock = GetDbSetMock(users);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<AuthorizedUser>(cancellationToken))
               .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await repository.GetUserByIdAsync("invalidUser", cancellationToken);
            // Assert
            Assert.IsNull(result);
        }
        [Test]
        public async Task AddUserAsync_ValidUser_ReturnsAddedUser()
        {
            // Arrange
            var newUser = new AuthorizedUser
            {
                Id = "newUser123",
                Name = "NewTestUser"
            };
            repositoryMock.Setup(repo => repo.AddAsync(newUser, cancellationToken)).ReturnsAsync(newUser);
            // Act
            var result = await repository.AddUserAsync(newUser, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(newUser.Id));
            Assert.That(result.Name, Is.EqualTo(newUser.Name));
            repositoryMock.Verify(repo => repo.AddAsync(newUser, cancellationToken), Times.Once);
        }
    }
}