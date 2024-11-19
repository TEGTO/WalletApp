using MediatR;
using WalletApi.Features.AuthFeature.Dtos;

namespace WalletApi.Features.AuthFeature.Command.RegisterUser
{
    public record RegisterUserCommand(UserRegistrationRequest Request) : IRequest<UserAuthenticationResponse>;
}
