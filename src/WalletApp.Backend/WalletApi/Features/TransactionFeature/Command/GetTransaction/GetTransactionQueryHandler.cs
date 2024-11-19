using AutoMapper;
using MediatR;
using WalletEntities.Domain.Dtos;
using WalletEntities.Repositories;

namespace WalletApi.Features.TransactionFeature.Command.GetTransaction
{
    public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionResponse?>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IMapper mapper;

        public GetTransactionQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.mapper = mapper;
        }

        public async Task<TransactionResponse?> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
        {
            var transaction = await transactionRepository.GetTransactionByIdAsync(request.UserId, request.Id, cancellationToken);
            return mapper.Map<TransactionResponse>(transaction);
        }
    }
}
