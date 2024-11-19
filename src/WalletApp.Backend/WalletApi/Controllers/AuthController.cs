using MediatR;
using Microsoft.AspNetCore.Mvc;
using WalletApi.Features.AuthFeature.Command.LoginUser;
using WalletApi.Features.AuthFeature.Command.RefreshToken;
using WalletApi.Features.AuthFeature.Command.RegisterUser;
using WalletApi.Features.AuthFeature.Dtos;

namespace WalletApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserAuthenticationResponse>> Register(UserRegistrationRequest request, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new RegisterUserCommand(request), cancellationToken);
            return CreatedAtAction(nameof(Register), new { id = response.Email }, response);
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserAuthenticationResponse>> Login(UserAuthenticationRequest request, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new LoginUserCommand(request), cancellationToken);
            return Ok(response);
        }
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthToken>> Refresh(AuthToken request, CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new RefreshTokenCommand(request), cancellationToken);
            return Ok(response);
        }
    }
}
