using AuthEntities.Domain.Entities;
using AutoMapper;
using ExceptionHandling;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WalletApi.Features.AuthFeature.Dtos;
using WalletApi.Features.AuthFeature.Services;

namespace WalletApi.Features.AuthFeature.Command.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserAuthenticationResponse>
    {
        private readonly IAuthService authService;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public RegisterUserCommandHandler(IAuthService authService, IUserService userService, IMapper mapper)
        {
            this.authService = authService;
            this.userService = userService;
            this.mapper = mapper;
        }

        public async Task<UserAuthenticationResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var user = mapper.Map<User>(request);

            var errors = new List<IdentityError>();

            var registerParams = new RegisterUserParams(user, request.Login, request.Password);
            errors.AddRange((await authService.RegisterUserAsync(registerParams, cancellationToken)).Errors);
            if (Utilities.HasErrors(errors, out var errorResponse)) throw new AuthorizationException(errorResponse);

            var loginResult = await userService.SetUserLoginAsync(user, request.Login, cancellationToken);
            if (loginResult != null && Utilities.HasErrors(loginResult.Errors, out errorResponse)) throw new AuthorizationException(errorResponse);

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
