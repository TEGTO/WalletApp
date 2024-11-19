using MediatR;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetBalance
{
    public record GetBalanceQuery() : IRequest<BalanceResponse>;
}
