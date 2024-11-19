using MediatR;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetPaymentDue
{
    public record GetPaymentDueQuery() : IRequest<PaymentDueResponse>;
}
