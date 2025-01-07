using Microsoft.AspNetCore.Http;
using Shared.Validation;

namespace Shared.Filters;

public class BadRequestFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
		try
		{
			return await next(ctx);
		}
		catch (Exception ex) when(ex is ArgumentException or ArgumentNullException or BadHttpRequestException)
		{
			return Results.Problem(ex.Message.GetProblemDetails());
		}
    }
}