using MediatR;
using WalletEntities.Domain.Dtos;

namespace WalletApi.Features.TransactionFeature.Command.GetTransaction
{
    public record GetTransactionQuery(string UserId, long Id) : IRequest<TransactionResponse?>;
}
