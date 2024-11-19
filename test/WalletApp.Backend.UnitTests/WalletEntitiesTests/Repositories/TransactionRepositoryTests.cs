using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using WalletEntities.Data;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories.Tests
{
    [TestFixture]
    internal class TransactionRepositoryTests
    {
        private Mock<IDatabaseRepository<WalletDbContext>> repositoryMock;
        private CancellationToken cancellationToken;
        private TransactionRepository transactionRepository;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IDatabaseRepository<WalletDbContext>>();
            transactionRepository = new TransactionRepository(repositoryMock.Object);
            cancellationToken = new CancellationToken();
        }

        private static Mock<DbSet<T>> GetDbSetMock<T>(List<T> data) where T : class
        {
            return data.AsQueryable().BuildMockDbSet();
        }

        [Test]
        public async Task GetTransactionByIdAsync_ValidIdAndUserId_ReturnsTransaction()
        {
            // Arrange
            var transaction = new Transaction
            {
                Id = 1,
                Card = new Card { Id = "card123", UserId = "user123" },
                Sum = 100,
                Name = "TestTransaction",
                Date = DateTime.UtcNow
            };
            var transactions = new List<Transaction> { transaction };
            var dbSetMock = GetDbSetMock(transactions);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<Transaction>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await transactionRepository.GetTransactionByIdAsync("user123", 1, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(transaction.Id));
            Assert.That(result.Card.UserId, Is.EqualTo("user123"));
            repositoryMock.Verify(repo => repo.GetQueryableAsync<Transaction>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetTransactionByIdAsync_InvalidIdOrUserId_ReturnsNull()
        {
            // Arrange
            var transactions = new List<Transaction>();
            var dbSetMock = GetDbSetMock(transactions);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<Transaction>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await transactionRepository.GetTransactionByIdAsync("user123", 999, cancellationToken);
            // Assert
            Assert.IsNull(result);
            repositoryMock.Verify(repo => repo.GetQueryableAsync<Transaction>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task AddTransactionAsync_ValidTransaction_ReturnsAddedTransaction()
        {
            // Arrange
            var newTransaction = new Transaction
            {
                Id = 1,
                Card = new Card { Id = "card123", UserId = "user123" },
                Sum = 100,
                Name = "NewTransaction",
                Date = DateTime.UtcNow
            };
            repositoryMock.Setup(repo => repo.AddAsync(newTransaction, cancellationToken)).ReturnsAsync(newTransaction);
            // Act
            var result = await transactionRepository.AddTransactionAsync(newTransaction, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(newTransaction.Id));
            Assert.That(result.Name, Is.EqualTo(newTransaction.Name));
            repositoryMock.Verify(repo => repo.AddAsync(newTransaction, cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetPaginatedAsync_ValidPagination_ReturnsPaginatedTransactions()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, Card = new Card { Id = "card123", UserId = "user123" }, Sum = 100, Date = DateTime.UtcNow },
                new Transaction { Id = 2, Card = new Card { Id = "card123", UserId = "user123" }, Sum = 200, Date = DateTime.UtcNow.AddDays(-1) },
                new Transaction { Id = 3, Card = new Card { Id = "card123", UserId = "user123" }, Sum = 300, Date = DateTime.UtcNow.AddDays(-2) }
            };
            var dbSetMock = GetDbSetMock(transactions);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<Transaction>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 2 };
            // Act
            var result = await transactionRepository.GetPaginatedAsync("user123", paginationRequest, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Id, Is.EqualTo(1));
            repositoryMock.Verify(repo => repo.GetQueryableAsync<Transaction>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetPaginatedAsync_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, Card = new Card { Id = "card123", UserId = "user123" } }
            };
            var dbSetMock = GetDbSetMock(transactions);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<Transaction>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            var paginationRequest = new PaginationRequest { PageNumber = 1, PageSize = 2 };
            // Act
            var result = await transactionRepository.GetPaginatedAsync("invalidUser", paginationRequest, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
            repositoryMock.Verify(repo => repo.GetQueryableAsync<Transaction>(cancellationToken), Times.Once);
        }
    }
}