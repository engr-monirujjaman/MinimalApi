using FluentValidation;

namespace WebApi.Requests;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty();
        
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Age)
            .GreaterThan(17);

    }
}