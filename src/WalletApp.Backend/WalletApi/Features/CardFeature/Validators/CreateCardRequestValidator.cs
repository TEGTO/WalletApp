using FluentValidation;
using WalletApi.Features.CardFeature.Dtos;

namespace WalletApi.Features.CardFeature.Validators
{
    public class CreateCardRequestValidator : AbstractValidator<CreateCardRequest>
    {
        public CreateCardRequestValidator()
        {
        }
    }
}
