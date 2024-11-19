using MediatR;
using WalletApi.Features.TransactionFeature.Dtos;
using WalletEntities.Domain.Dtos;

namespace WalletApi.Features.TransactionFeature.Command.CreateTransaction
{
    public record CreateTransactionCommand(string UserId, CreateTransactionRequest Request) : IRequest<TransactionResponse>;
}
