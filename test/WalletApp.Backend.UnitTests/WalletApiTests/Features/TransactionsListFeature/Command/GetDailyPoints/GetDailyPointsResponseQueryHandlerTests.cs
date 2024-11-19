using WalletApi.Features.TransactionsListFeature.Command.GetBalance;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetDailyPoints.Tests
{
    [TestFixture]
    internal class GetDailyPointsResponseQueryHandlerTests
    {
        private GetBalanceQueryHandler handler;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            handler = new GetBalanceQueryHandler();
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsBalanceResponse()
        {
            // Arrange
            var query = new GetBalanceQuery();
            // Act
            var result = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.CardLimit, Is.EqualTo(1500));
            Assert.That(result.CardBalance, Is.InRange(0, 1500));
            Assert.That(result.AvailableBalance, Is.InRange(0, 1500));
            Assert.That(result.AvailableBalance + result.CardBalance, Is.EqualTo(result.CardLimit));
        }
        [Test]
        public async Task Handle_ValidRequest_ReturnsRoundedValues()
        {
            // Arrange
            var query = new GetBalanceQuery();
            // Act
            var result = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.CardBalance, Is.EqualTo(Math.Round(result.CardBalance, 2)));
            Assert.That(result.AvailableBalance, Is.EqualTo(Math.Round(result.AvailableBalance, 2)));
        }
        [Test]
        public async Task Handle_MultipleRequests_ReturnsRandomBalances()
        {
            // Arrange
            var query = new GetBalanceQuery();
            var results = new List<BalanceResponse>();
            // Act
            for (int i = 0; i < 10; i++)
            {
                results.Add(await handler.Handle(query, cancellationToken));
            }
            // Assert
            Assert.That(results.Select(r => r.CardBalance).Distinct().Count(), Is.GreaterThan(1));
        }
    }
}