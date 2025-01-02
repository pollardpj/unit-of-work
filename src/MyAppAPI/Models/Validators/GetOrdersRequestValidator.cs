using FluentValidation;

namespace MyAppAPI.Models.Validators;

public class GetOrdersRequestValidator : AbstractValidator<GetOrdersRequest>
{
    public GetOrdersRequestValidator()
    {
        RuleFor(r => r.Top)
            .NotEmpty()
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);

        RuleFor(r => r.Skip)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);
    }
}
