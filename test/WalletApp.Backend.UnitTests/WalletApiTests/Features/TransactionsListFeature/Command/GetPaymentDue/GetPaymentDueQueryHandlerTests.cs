using System.Globalization;
using System.Reflection;

namespace WalletApi.Features.TransactionsListFeature.Command.GetPaymentDue.Tests
{
    [TestFixture]
    internal class GetPaymentDueQueryHandlerTests
    {
        private GetPaymentDueQueryHandler handler;
        private CancellationToken cancellationToken;

        [SetUp]
        public void SetUp()
        {
            handler = new GetPaymentDueQueryHandler();
            cancellationToken = new CancellationToken();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsPaymentDueResponse()
        {
            // Arrange
            var query = new GetPaymentDueQuery();
            var currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
            var expectedText = $"You’ve paid your {currentMonth} balance.";
            // Act
            var response = await handler.Handle(query, cancellationToken);
            // Assert
            Assert.IsNotNull(response);
            Assert.That(response.DueText, Is.EqualTo(expectedText), "The due text should reflect the current month's payment status.");
        }
        [Test]
        public void GetDueText_CorrectMonth_ReturnsExpectedText()
        {
            // Arrange
            var currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
            var expectedText = $"You’ve paid your {currentMonth} balance.";
            // Act
            var result = InvokePrivateMethod<string>(handler, "GetDueText");
            // Assert
            Assert.That(result, Is.EqualTo(expectedText));
        }
        private T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
        {
            var methodInfo = instance.GetType()
                                     .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)methodInfo.Invoke(instance, parameters);
        }
    }
}