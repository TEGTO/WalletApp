using DatabaseControl.Repositories;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using WalletEntities.Data;
using WalletEntities.Domain.Entities;

namespace WalletEntities.Repositories.Tests
{
    [TestFixture]
    internal class CardRepositoryTests
    {
        private Mock<IDatabaseRepository<WalletDbContext>> repositoryMock;
        private CancellationToken cancellationToken;
        private CardRepository cardRepository;

        [SetUp]
        public void SetUp()
        {
            repositoryMock = new Mock<IDatabaseRepository<WalletDbContext>>();
            cardRepository = new CardRepository(repositoryMock.Object);
            cancellationToken = new CancellationToken();
        }

        private static Mock<DbSet<T>> GetDbSetMock<T>(List<T> data) where T : class
        {
            return data.AsQueryable().BuildMockDbSet();
        }

        [Test]
        public async Task GetCardByIdAsync_ValidId_ReturnsCard()
        {
            // Arrange
            var card = new Card
            {
                Id = "card123",
                UserId = "UserId"
            };
            var cards = new List<Card> { card };
            var dbSetMock = GetDbSetMock(cards);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<Card>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await cardRepository.GetCardByIdAsync("card123", cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.Id, Is.EqualTo(card.Id));
            Assert.That(result.UserId, Is.EqualTo(card.UserId));
            repositoryMock.Verify(repo => repo.GetQueryableAsync<Card>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task GetCardByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            var cards = new List<Card>();
            var dbSetMock = GetDbSetMock(cards);
            repositoryMock.Setup(repo => repo.GetQueryableAsync<Card>(cancellationToken))
                .ReturnsAsync(dbSetMock.Object);
            // Act
            var result = await cardRepository.GetCardByIdAsync("invalidCardId", cancellationToken);
            // Assert
            Assert.IsNull(result);
            repositoryMock.Verify(repo => repo.GetQueryableAsync<Card>(cancellationToken), Times.Once);
        }
        [Test]
        public async Task AddCardAsync_ValidCard_ReturnsAddedCard()
        {
            // Arrange
            var newCard = new Card
            {
                Id = "card123",
                UserId = "UserId"
            };
            repositoryMock.Setup(repo => repo.AddAsync(newCard, cancellationToken)).ReturnsAsync(newCard);
            // Act
            var result = await cardRepository.AddCardAsync(newCard, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(newCard.Id));
            Assert.That(result.UserId, Is.EqualTo(newCard.UserId));
            repositoryMock.Verify(repo => repo.AddAsync(newCard, cancellationToken), Times.Once);
        }
    }
}