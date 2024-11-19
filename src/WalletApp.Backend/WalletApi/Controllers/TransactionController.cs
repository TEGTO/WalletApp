
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletApi.Features.TransactionFeature.Command.CreateTransaction;
using WalletApi.Features.TransactionFeature.Command.GetTransaction;
using WalletApi.Features.TransactionFeature.Dtos;
using WalletEntities.Domain.Dtos;

namespace WalletApi.Controllers
{
    [Authorize]
    [Route("transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator mediator;

        public TransactionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction(CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await mediator.Send(new CreateTransactionCommand(userId, request), cancellationToken);
            return CreatedAtAction(nameof(CreateTransaction), new { id = response.Id }, response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetTransaction([FromRoute] long id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await mediator.Send(new GetTransactionQuery(userId, id), cancellationToken);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
