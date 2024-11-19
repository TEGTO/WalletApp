using AutoMapper;
using Moq;
using WalletApi.Features.CardFeature.Dtos;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using WalletEntities.Repositories;

namespace WalletApi.Features.CardFeature.Command.CreateCard.Tests
{
    [TestFixture]
    internal class CreateCardCommandHandlerTests
    {
        private Mock<IAuthorizedUserRepository> authorizedUserRepositoryMock;
        private Mock<ICardRepository> cardRepositoryMock;
        private Mock<IMapper> mapperMock;
        private CreateCardCommandHandler handler;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            authorizedUserRepositoryMock = new Mock<IAuthorizedUserRepository>();
            cardRepositoryMock = new Mock<ICardRepository>();
            mapperMock = new Mock<IMapper>();
            handler = new CreateCardCommandHandler(authorizedUserRepositoryMock.Object, cardRepositoryMock.Object, mapperMock.Object);
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_UserExists_AddsCardAndReturnsResponse()
        {
            // Arrange
            var userId = "user123";
            var userName = "TestUser";
            var request = new CreateCardRequest { };
            var command = new CreateCardCommand(userId, userName, request);
            var user = new AuthorizedUser { Id = userId, Name = userName };
            var card = new Card { Id = "card123", UserId = userId };
            var response = new CardResponse { Id = "card123", UserId = userId };
            authorizedUserRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId, cancellationToken)).ReturnsAsync(user);
            mapperMock.Setup(mapper => mapper.Map<Card>(request)).Returns(card);
            cardRepositoryMock.Setup(repo => repo.AddCardAsync(card, cancellationToken)).ReturnsAsync(card);
            mapperMock.Setup(mapper => mapper.Map<CardResponse>(card)).Returns(response);
            // Act
            var result = await handler.Handle(command, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo("card123"));
            Assert.That(result.UserId, Is.EqualTo(userId));
            authorizedUserRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId, cancellationToken), Times.Once);
            authorizedUserRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<AuthorizedUser>(), cancellationToken), Times.Never);
            cardRepositoryMock.Verify(repo => repo.AddCardAsync(card, cancellationToken), Times.Once);
        }
        [Test]
        public async Task Handle_UserDoesNotExist_CreatesUserAndAddsCard()
        {
            // Arrange
            var userId = "user123";
            var userName = "NewUser";
            var request = new CreateCardRequest { };
            var command = new CreateCardCommand(userId, userName, request);
            AuthorizedUser? nullUser = null;
            var newUser = new AuthorizedUser { Id = userId, Name = userName };
            var card = new Card { Id = "card123", UserId = userId };
            var response = new CardResponse { Id = "card123", UserId = userId };
            authorizedUserRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId, cancellationToken)).ReturnsAsync(nullUser);
            authorizedUserRepositoryMock.Setup(repo => repo.AddUserAsync(It.Is<AuthorizedUser>(u => u.Id == userId && u.Name == userName), cancellationToken)).ReturnsAsync(newUser);
            mapperMock.Setup(mapper => mapper.Map<Card>(request)).Returns(card);
            cardRepositoryMock.Setup(repo => repo.AddCardAsync(card, cancellationToken)).ReturnsAsync(card);
            mapperMock.Setup(mapper => mapper.Map<CardResponse>(card)).Returns(response);
            // Act
            var result = await handler.Handle(command, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo("card123"));
            Assert.That(result.UserId, Is.EqualTo(userId));
            authorizedUserRepositoryMock.Verify(repo => repo.GetUserByIdAsync(userId, cancellationToken), Times.Once);
            authorizedUserRepositoryMock.Verify(repo => repo.AddUserAsync(It.Is<AuthorizedUser>(u => u.Id == userId && u.Name == userName), cancellationToken), Times.Once);
            cardRepositoryMock.Verify(repo => repo.AddCardAsync(card, cancellationToken), Times.Once);
        }
    }
}