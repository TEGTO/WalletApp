using MediatR;
using WalletEntities.Domain.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetTransactions
{
    public record GetTransactionsQuery(string UserId) : IRequest<IEnumerable<TransactionResponse>>;
}
