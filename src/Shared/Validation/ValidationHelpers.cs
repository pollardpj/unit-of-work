using Microsoft.AspNetCore.Http;

namespace Shared.Validation;

public static class ValidationHelpers
{
    public static HttpValidationProblemDetails GetProblemDetails(this string error)
    {
        return new HttpValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = new Dictionary<string, string[]>
            {
                ["Validation"] = [error]
            }
        };
    }
}
