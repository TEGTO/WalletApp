using Authentication.Models;
using AutoMapper;
using MediatR;
using WalletApi.Features.AuthFeature.Dtos;
using WalletApi.Features.AuthFeature.Services;

namespace WalletApi.Features.AuthFeature.Command.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthToken>
    {
        private readonly IAuthService authService;
        private readonly IMapper mapper;

        public RefreshTokenCommandHandler(IAuthService authService, IMapper mapper)
        {
            this.authService = authService;
            this.mapper = mapper;
        }

        public async Task<AuthToken> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var tokenData = mapper.Map<AccessTokenData>(command.Request);
            var newToken = await authService.RefreshTokenAsync(tokenData, cancellationToken);
            return mapper.Map<AuthToken>(newToken);
        }
    }
}
