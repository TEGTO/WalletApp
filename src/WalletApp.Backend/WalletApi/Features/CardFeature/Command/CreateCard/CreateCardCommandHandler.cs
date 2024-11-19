using AutoMapper;
using MediatR;
using WalletEntities.Domain.Dtos;
using WalletEntities.Domain.Entities;
using WalletEntities.Repositories;

namespace WalletApi.Features.CardFeature.Command.CreateCard
{
    public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CardResponse>
    {
        private readonly IAuthorizedUserRepository authorizedUserRepository;
        private readonly ICardRepository cardRepository;
        private readonly IMapper mapper;

        public CreateCardCommandHandler(IAuthorizedUserRepository authorizedUserRepository, ICardRepository cardRepository, IMapper mapper)
        {
            this.authorizedUserRepository = authorizedUserRepository;
            this.cardRepository = cardRepository;
            this.mapper = mapper;
        }

        public async Task<CardResponse> Handle(CreateCardCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var user = await authorizedUserRepository.GetUserByIdAsync(command.UserId, cancellationToken);

            if (user == null)
            {
                user = new AuthorizedUser() { Id = command.UserId, Name = command.UserName };
                await authorizedUserRepository.AddUserAsync(user, cancellationToken);
            }

            var card = mapper.Map<Card>(request);
            card.UserId = user.Id;

            card = await cardRepository.AddCardAsync(card, cancellationToken);

            return mapper.Map<CardResponse>(card);
        }
    }
}
