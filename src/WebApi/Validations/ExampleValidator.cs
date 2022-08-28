using FluentValidation;
using WebApi.Requests;

namespace WebApi.Validations;

public class ExampleValidator : AbstractValidator<ExampleRequest>
{
    public ExampleValidator()
    {
        RuleFor(x => x.Age)
            .GreaterThan(17)
            .WithMessage("Age must be greater then 17");

        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty();
    }
}