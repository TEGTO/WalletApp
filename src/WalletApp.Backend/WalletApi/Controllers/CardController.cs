using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletApi.Features.CardFeature.Command.CreateCard;
using WalletApi.Features.CardFeature.Dtos;
using WalletEntities.Domain.Dtos;

namespace WalletApi.Controllers
{
    [Authorize]
    [Route("card")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly IMediator mediator;

        public CardController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<CardResponse>> CreateCard(CreateCardRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);

            var response = await mediator.Send(new CreateCardCommand(userId, userName, request), cancellationToken);
            return CreatedAtAction(nameof(CreateCard), new { id = response.Id }, response);
        }
    }
}
