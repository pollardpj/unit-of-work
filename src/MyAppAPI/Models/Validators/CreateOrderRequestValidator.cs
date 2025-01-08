using FluentValidation;

namespace MyAppAPI.Models.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(r => r.ProductName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(r => r.Email)
            .NotEmpty()
            .MaximumLength(255)
            .EmailAddress();
    }
}

