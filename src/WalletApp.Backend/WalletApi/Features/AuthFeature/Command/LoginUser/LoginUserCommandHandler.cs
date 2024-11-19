using AutoMapper;
using MediatR;
using WalletApi.Features.AuthFeature.Dtos;
using WalletApi.Features.AuthFeature.Services;

namespace WalletApi.Features.AuthFeature.Command.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserAuthenticationResponse>
    {
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public LoginUserCommandHandler(IAuthService authService, IUserService userService, IMapper mapper)
        {
            this.authService = authService;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<UserAuthenticationResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var user = await userService.GetUserByLoginAsync(request.Login, cancellationToken);
            if (user == null) throw new UnauthorizedAccessException("Invalid authentication! Wrong password or login!");

            var loginParams = new LoginUserParams(user, request.Password);
            var token = await authService.LoginUserAsync(loginParams, cancellationToken);

            var tokenDto = mapper.Map<AuthToken>(token);

            return new UserAuthenticationResponse
            {
                AuthToken = tokenDto,
                UserId = user.Id,
                Email = user.Email
            };
        }
    }
}
