using FluentValidation;

namespace MyAppAPI.Models.Validators;

public class OrderRequestValidator : AbstractValidator<OrderRequest>
{
    public OrderRequestValidator()
    {
        RuleFor(r => r.ProductName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(r => r.Reference)
            .NotEmpty();
    }
}

