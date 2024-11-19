using MediatR;
using WalletApi.Features.AuthFeature.Dtos;

namespace WalletApi.Features.AuthFeature.Command.RefreshToken
{
    public record RefreshTokenCommand(AuthToken Request) : IRequest<AuthToken>;
}
