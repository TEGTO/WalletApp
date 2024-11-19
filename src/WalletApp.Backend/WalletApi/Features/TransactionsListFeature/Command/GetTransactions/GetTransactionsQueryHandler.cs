using AutoMapper;
using MediatR;
using WalletEntities.Domain.Dtos;
using WalletEntities.Repositories;

namespace WalletApi.Features.TransactionsListFeature.Command.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionResponse>>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IMapper mapper;

        public GetTransactionsQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<TransactionResponse>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            var pagRequest = new PaginationRequest() { PageNumber = 1, PageSize = 10 };
            var transactions = await transactionRepository.GetPaginatedAsync(request.UserId, pagRequest, cancellationToken);

            return transactions.Select(mapper.Map<TransactionResponse>);
        }
    }
}