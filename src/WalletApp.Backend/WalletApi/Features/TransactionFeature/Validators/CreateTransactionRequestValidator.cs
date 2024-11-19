using FluentValidation;
using WalletApi.Features.TransactionFeature.Dtos;

namespace WalletApi.Features.TransactionFeature.Validators
{
    public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
    {
        public CreateTransactionRequestValidator()
        {
            RuleFor(x => x.CardId).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Sum).NotNull().NotEmpty().GreaterThan(0);
            RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(2048);
            RuleFor(x => x.Date).NotNull().NotEmpty().LessThanOrEqualTo(DateTime.UtcNow);
            RuleFor(x => x.Pending).NotNull().NotEmpty();
            RuleFor(x => x.AuthorizedUserId).MaximumLength(256).When(x => !string.IsNullOrEmpty(x.AuthorizedUserId));
        }
    }
}
