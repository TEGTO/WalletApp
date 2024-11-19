using MediatR;
using WalletApi.Features.AuthFeature.Dtos;

namespace WalletApi.Features.AuthFeature.Command.LoginUser
{
    public record LoginUserCommand(UserAuthenticationRequest Request) : IRequest<UserAuthenticationResponse>;
}
