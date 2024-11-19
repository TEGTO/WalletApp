using FluentValidation;
using WalletApi.Features.AuthFeature.Dtos;

namespace WalletApi.Features.AuthFeature.Validators
{
    public class UserRegistrationRequestValidator : AbstractValidator<UserRegistrationRequest>
    {
        public UserRegistrationRequestValidator()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Login).NotNull().NotEmpty().MaximumLength(256);
            RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(8).MaximumLength(256);
            RuleFor(x => x.ConfirmPassword).Must((model, field) => field == model.Password)
                .WithMessage("Passwords do not match.").MaximumLength(256);
        }
    }
}