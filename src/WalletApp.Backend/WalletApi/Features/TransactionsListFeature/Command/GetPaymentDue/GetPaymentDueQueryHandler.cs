using MediatR;
using System.Globalization;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetPaymentDue
{
    public class GetPaymentDueQueryHandler : IRequestHandler<GetPaymentDueQuery, PaymentDueResponse>
    {
        public Task<PaymentDueResponse> Handle(GetPaymentDueQuery request, CancellationToken cancellationToken)
        {
            var text = GetDueText();
            return Task.FromResult(new PaymentDueResponse() { DueText = text });
        }

        private string GetDueText()
        {
            var currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.UtcNow.Month);
            return $"You’ve paid your {currentMonth} balance.";
        }
    }
}
