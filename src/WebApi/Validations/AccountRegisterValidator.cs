using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validations;

public class AccountRegisterValidator : AbstractValidator<AccountRegisterRequest>
{
    public AccountRegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();

        When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
        {
            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Confirm password miss matched.");
        });

        RuleFor(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty();
    }
}