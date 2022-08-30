using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validations;

public class AccountLoginValidator : AbstractValidator<AccountLoginRequest>
{
    public AccountLoginValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();
    }
}