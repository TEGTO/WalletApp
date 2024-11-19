using AutoMapper;
using Moq;
using WalletApi.Features.TransactionFeature.Dtos;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using WalletEntities.Repositories;

namespace WalletApi.Features.TransactionFeature.Command.CreateTransaction.Tests
{
    [TestFixture]
    internal class CreateTransactionCommandHandlerTests
    {
        private Mock<ITransactionRepository> transactionRepositoryMock;
        private Mock<IAuthorizedUserRepository> authorizedUserRepositoryMock;
        private Mock<ICardRepository> cardRepositoryMock;
        private Mock<IMapper> mapperMock;
        private CreateTransactionCommandHandler handler;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            transactionRepositoryMock = new Mock<ITransactionRepository>();
            authorizedUserRepositoryMock = new Mock<IAuthorizedUserRepository>();
            cardRepositoryMock = new Mock<ICardRepository>();
            mapperMock = new Mock<IMapper>();
            handler = new CreateTransactionCommandHandler(
                transactionRepositoryMock.Object,
                authorizedUserRepositoryMock.Object,
                cardRepositoryMock.Object,
                mapperMock.Object);
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_ValidCommand_AddsTransactionAndReturnsResponse()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                "user123",
                new CreateTransactionRequest
                {
                    CardId = "card123",
                    Type = TransactionType.Payment,
                    Sum = 100,
                    Name = "Test Transaction",
                    Description = "Description",
                    Date = DateTime.UtcNow,
                    Pending = false,
                    AuthorizedUserId = "authUser123"
                });
            var card = new Card { Id = "card123", UserId = "user123" };
            var user = new AuthorizedUser { Id = "authUser123", Name = "Authorized User" };
            var transaction = new Transaction { Id = 1, CardId = "card123", Sum = 100, Name = "Test Transaction" };
            var transactionResponse = new TransactionResponse
            {
                Id = 1,
                Name = "Test Transaction",
                Sum = 100
            };
            cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(command.Request.CardId, cancellationToken))
                .ReturnsAsync(card);
            authorizedUserRepositoryMock.Setup(repo => repo.GetUserByIdAsync(command.Request.AuthorizedUserId, cancellationToken))
                .ReturnsAsync(user);
            mapperMock.Setup(mapper => mapper.Map<Transaction>(command.Request))
                .Returns(transaction);
            transactionRepositoryMock.Setup(repo => repo.AddTransactionAsync(transaction, cancellationToken))
                .ReturnsAsync(transaction);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionByIdAsync(command.UserId, transaction.Id, cancellationToken))
                .ReturnsAsync(transaction);
            mapperMock.Setup(mapper => mapper.Map<TransactionResponse>(transaction))
                .Returns(transactionResponse);
            // Act
            var result = await handler.Handle(command, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test Transaction"));
            cardRepositoryMock.Verify(repo => repo.GetCardByIdAsync(command.Request.CardId, cancellationToken), Times.Once);
            authorizedUserRepositoryMock.Verify(repo => repo.GetUserByIdAsync(command.Request.AuthorizedUserId, cancellationToken), Times.Once);
            transactionRepositoryMock.Verify(repo => repo.AddTransactionAsync(transaction, cancellationToken), Times.Once);
        }
        [Test]
        public void Handle_InvalidCardId_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                "user123",
                new CreateTransactionRequest
                {
                    CardId = "invalidCardId",
                    Type = TransactionType.Payment,
                    Sum = 100,
                    Name = "Test Transaction",
                    Description = "Description",
                    Date = DateTime.UtcNow,
                    Pending = false
                });
            cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(command.Request.CardId, cancellationToken))
                .ReturnsAsync((Card?)null);
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.Handle(command, cancellationToken));
            cardRepositoryMock.Verify(repo => repo.GetCardByIdAsync(command.Request.CardId, cancellationToken), Times.Once);
        }
        [Test]
        public void Handle_InvalidAuthorizedUserId_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new CreateTransactionCommand(
                "user123",
                new CreateTransactionRequest
                {
                    CardId = "card123",
                    Type = TransactionType.Payment,
                    Sum = 100,
                    Name = "Test Transaction",
                    Description = "Description",
                    Date = DateTime.UtcNow,
                    Pending = false,
                    AuthorizedUserId = "invalidAuthUser"
                });
            var card = new Card { Id = "card123", UserId = "user123" };
            cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(command.Request.CardId, cancellationToken))
                .ReturnsAsync(card);
            authorizedUserRepositoryMock.Setup(repo => repo.GetUserByIdAsync(command.Request.AuthorizedUserId, cancellationToken))
                .ReturnsAsync((AuthorizedUser?)null);
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await handler.Handle(command, cancellationToken));
            cardRepositoryMock.Verify(repo => repo.GetCardByIdAsync(command.Request.CardId, cancellationToken), Times.Once);
            authorizedUserRepositoryMock.Verify(repo => repo.GetUserByIdAsync(command.Request.AuthorizedUserId, cancellationToken), Times.Once);
        }
    }
}