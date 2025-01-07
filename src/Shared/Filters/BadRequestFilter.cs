using Microsoft.AspNetCore.Http;
using Shared.Exceptions;
using Shared.Validation;

namespace Shared.Filters;

public class BadRequestFilter : IEndpointFilter
{
    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
		try
		{
			var result = await next(ctx);

            if (ctx.HttpContext.Response.StatusCode == 409 &&
				ctx.HttpContext.Response.ContentLength == null)
            {
				if (result is string conflictMessage)
				{
					return Results.Conflict(conflictMessage.GetProblemDetails());
				}

				return Results.Conflict();
            }

            if (ctx.HttpContext.Response.StatusCode == 400 && 
				ctx.HttpContext.Response.ContentLength == null && 
				result is string badrequestMessage)
			{
				return Results.Problem(badrequestMessage.GetProblemDetails());
			}

			return result;
		}
		catch (ConflictException)
		{
            return Results.Conflict();
        }
		catch (Exception ex) when(ex is ArgumentException or 
										ArgumentNullException or 
										BadHttpRequestException or
										BadRequestException)
		{
			return Results.Problem(ex.Message.GetProblemDetails());
		}
    }
}