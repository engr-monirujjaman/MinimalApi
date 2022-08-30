using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validations;

public class SecretValidator : AbstractValidator<SecretRequest>
{
    public SecretValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();
    }
}