using AutoMapper;
using MediatR;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using WalletEntities.Repositories;

namespace WalletApi.Features.TransactionFeature.Command.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionResponse>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IAuthorizedUserRepository authorizedUserRepository;
        private readonly ICardRepository cardRepository;
        private readonly IMapper mapper;

        public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IAuthorizedUserRepository authorizedUserRepository, ICardRepository cardRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.authorizedUserRepository = authorizedUserRepository;
            this.cardRepository = cardRepository;
            this.mapper = mapper;
        }

        public async Task<TransactionResponse> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var card = await cardRepository.GetCardByIdAsync(request.CardId, cancellationToken);

            if (card == null)
            {
                throw new InvalidOperationException("Card is not found!");
            }

            var transaction = mapper.Map<Transaction>(request);

            AuthorizedUser? user = null;

            if (request.AuthorizedUserId != null)
            {
                user = await authorizedUserRepository.GetUserByIdAsync(request.AuthorizedUserId, cancellationToken);

                if (user == null)
                {
                    throw new InvalidOperationException("User is not found!");
                }
            }

            var addedTransaction = await transactionRepository.AddTransactionAsync(transaction, cancellationToken);

            transaction = await transactionRepository.GetTransactionByIdAsync(command.UserId, addedTransaction.Id, cancellationToken);

            return mapper.Map<TransactionResponse>(transaction);
        }
    }
}
