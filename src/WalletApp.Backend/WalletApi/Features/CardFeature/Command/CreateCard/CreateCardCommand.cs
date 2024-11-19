using MediatR;
using WalletApi.Features.CardFeature.Dtos;
using WalletEntities.Domain.Dtos;

namespace WalletApi.Features.CardFeature.Command.CreateCard
{
    public record CreateCardCommand(string UserId, string UserName, CreateCardRequest Request) : IRequest<CardResponse>;
}
