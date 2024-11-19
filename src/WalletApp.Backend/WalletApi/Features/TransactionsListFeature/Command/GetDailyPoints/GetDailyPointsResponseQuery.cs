using MediatR;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetDailyPoints
{
    public record GetDailyPointsResponseQuery() : IRequest<DailyPointsResponse>;
}
