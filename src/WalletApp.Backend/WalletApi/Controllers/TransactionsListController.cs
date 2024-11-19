using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WalletApi.Features.TransactionsListFeature.Command.GetBalance;
using WalletApi.Features.TransactionsListFeature.Command.GetDailyPoints;
using WalletApi.Features.TransactionsListFeature.Command.GetPaymentDue;
using WalletApi.Features.TransactionsListFeature.Command.GetTransactions;
using WalletApi.Features.TransactionsListFeature.Dtos;

namespace WalletApi.Controllers
{
    [Authorize]
    [Route("transactionslist")]
    [ApiController]
    public class TransactionsListController : ControllerBase
    {
        private readonly IMediator mediator;

        public TransactionsListController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<TransactionsListResponse>> GetTransactionsList(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var balanceTask = mediator.Send(new GetBalanceQuery(), cancellationToken);
            var paymentDueTask = mediator.Send(new GetPaymentDueQuery(), cancellationToken);
            var pointsTask = mediator.Send(new GetDailyPointsResponseQuery(), cancellationToken);
            var transactionsTask = mediator.Send(new GetTransactionsQuery(userId), cancellationToken);

            await Task.WhenAll(balanceTask, paymentDueTask, pointsTask, transactionsTask);

            var response = new TransactionsListResponse
            {
                CardBalance = balanceTask.Result.CardBalance,
                AvailableBalance = balanceTask.Result.AvailableBalance,
                CardLimit = balanceTask.Result.CardLimit,
                Points = pointsTask.Result.Points,
                DueText = paymentDueTask.Result.DueText,
                Transactions = transactionsTask.Result
            };

            return Ok(response);
        }
    }
}
