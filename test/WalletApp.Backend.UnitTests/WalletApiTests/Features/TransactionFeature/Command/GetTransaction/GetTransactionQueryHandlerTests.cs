using AutoMapper;
using Moq;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using WalletEntities.Repositories;

namespace WalletApi.Features.TransactionFeature.Command.GetTransaction.Tests
{
    [TestFixture]
    internal class GetTransactionQueryHandlerTests
    {
        private Mock<ITransactionRepository> transactionRepositoryMock;
        private Mock<IMapper> mapperMock;
        private GetTransactionQueryHandler handler;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            transactionRepositoryMock = new Mock<ITransactionRepository>();
            mapperMock = new Mock<IMapper>();
            handler = new GetTransactionQueryHandler(transactionRepositoryMock.Object, mapperMock.Object);
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsTransactionResponse()
        {
            // Arrange
            var userId = "user123";
            var transactionId = 1L;
            var query = new GetTransactionQuery(userId, transactionId);
            var transaction = new Transaction
            {
                Id = transactionId,
                Name = "Sample Transaction",
                Sum = 100.00M
            };
            var transactionResponse = new TransactionResponse
            {
                Id = transactionId,
                Name = "Sample Transaction",
                Sum = 100.00M
            };
            transactionRepositoryMock
                .Setup(repo => repo.GetTransactionByIdAsync(userId, transactionId, cancellationToken))
                .ReturnsAsync(transaction);
            mapperMock
                .Setup(mapper => mapper.Map<TransactionResponse>(transaction))
                .Returns(transactionResponse);
            // Act
            var result = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(transactionId));
            Assert.That(result.Name, Is.EqualTo("Sample Transaction"));
            transactionRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(userId, transactionId, cancellationToken), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<TransactionResponse>(transaction), Times.Once);
        }
        [Test]
        public async Task Handle_TransactionNotFound_ReturnsNull()
        {
            // Arrange
            var userId = "user123";
            var transactionId = 1L;
            var query = new GetTransactionQuery(userId, transactionId);
            transactionRepositoryMock
                .Setup(repo => repo.GetTransactionByIdAsync(userId, transactionId, cancellationToken))
                .ReturnsAsync((Transaction?)null);
            // Act
            var result = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNull(result);
            transactionRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(userId, transactionId, cancellationToken), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<TransactionResponse>(It.IsAny<Transaction>()), Times.Once);
        }
        [Test]
        public void Handle_RepositoryThrowsException_ThrowsSameException()
        {
            // Arrange
            var userId = "user123";
            var transactionId = 1L;
            var query = new GetTransactionQuery(userId, transactionId);
            transactionRepositoryMock
                .Setup(repo => repo.GetTransactionByIdAsync(userId, transactionId, cancellationToken))
                .ThrowsAsync(new InvalidOperationException("Database error"));
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await handler.Handle(query, cancellationToken));
            transactionRepositoryMock.Verify(repo => repo.GetTransactionByIdAsync(userId, transactionId, cancellationToken), Times.Once);
            mapperMock.Verify(mapper => mapper.Map<TransactionResponse>(It.IsAny<Transaction>()), Times.Never);
        }
    }
}