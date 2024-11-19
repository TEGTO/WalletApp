using AutoMapper;
using Moq;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using WalletEntities.Repositories;

namespace WalletApi.Features.TransactionsListFeature.Command.GetTransactions.Tests
{
    [TestFixture]
    internal class GetTransactionsQueryHandlerTests
    {
        private Mock<ITransactionRepository> transactionRepositoryMock;
        private Mock<IMapper> mapperMock;
        private GetTransactionsQueryHandler handler;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            transactionRepositoryMock = new Mock<ITransactionRepository>();
            mapperMock = new Mock<IMapper>();
            handler = new GetTransactionsQueryHandler(transactionRepositoryMock.Object, mapperMock.Object);
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsMappedTransactions()
        {
            // Arrange
            var userId = "user123";
            var query = new GetTransactionsQuery(userId);
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1, Name = "Transaction 1" },
                new Transaction { Id = 2, Name = "Transaction 2" }
            };
            var transactionResponses = new List<TransactionResponse>
            {
                new TransactionResponse { Id = 1, Name = "Transaction 1" },
                new TransactionResponse { Id = 2, Name = "Transaction 2" }
            };
            transactionRepositoryMock
                .Setup(repo => repo.GetPaginatedAsync(userId, It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactions);
            mapperMock
                .Setup(mapper => mapper.Map<TransactionResponse>(It.IsAny<Transaction>()))
                .Returns((Transaction transaction) =>
                    transactionResponses.FirstOrDefault(r => r.Id == transaction.Id));
            // Act
            var result = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Transaction 1"));
            Assert.That(result.Last().Name, Is.EqualTo("Transaction 2"));
            transactionRepositoryMock.Verify(repo => repo.GetPaginatedAsync(userId, It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Test]
        public async Task Handle_EmptyTransactions_ReturnsEmptyEnumerable()
        {
            // Arrange
            var userId = "user123";
            var query = new GetTransactionsQuery(userId);
            transactionRepositoryMock
                .Setup(repo => repo.GetPaginatedAsync(userId, It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Transaction>());
            // Act
            var result = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result, Is.Empty);
            transactionRepositoryMock.Verify(repo => repo.GetPaginatedAsync(userId, It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<TransactionResponse>(It.IsAny<Transaction>()), Times.Never);
        }
        [Test]
        public void Handle_RepositoryThrowsException_ThrowsSameException()
        {
            // Arrange
            var userId = "user123";
            var query = new GetTransactionsQuery(userId);
            transactionRepositoryMock
                .Setup(repo => repo.GetPaginatedAsync(userId, It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(query, cancellationToken));
            transactionRepositoryMock.Verify(repo => repo.GetPaginatedAsync(userId, It.IsAny<PaginationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<TransactionResponse>(It.IsAny<Transaction>()), Times.Never);
        }
    }
}