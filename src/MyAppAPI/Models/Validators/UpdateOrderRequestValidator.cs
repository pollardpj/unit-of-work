using FluentValidation;

namespace MyAppAPI.Models.Validators;

public class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator()
    {
        RuleFor(r => r.ProductName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(r => r.RowVersion)
            .NotEmpty()
            .Must(IsBase64String)
            .WithMessage("Must be a valid base64 string");
    }

    public static bool IsBase64String(string base64)
    {
        var buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out int _);
    }
}

