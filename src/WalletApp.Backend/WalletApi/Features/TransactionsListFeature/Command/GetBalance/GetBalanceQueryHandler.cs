using MediatR;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Features.TransactionsListFeature.Command.GetBalance
{
    public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, BalanceResponse>
    {
        public Task<BalanceResponse> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
        {
            var random = new Random();

            decimal balance = (decimal)(random.NextDouble() * 1500);
            balance = Math.Round(balance, 2);

            decimal limit = 1500;
            decimal available = limit - balance;

            return Task.FromResult(new BalanceResponse()
            {
                AvailableBalance = available,
                CardBalance = balance,
                CardLimit = limit
            });
        }
    }
}
