using Microsoft.AspNetCore.Http;
using Shared.Exceptions;

namespace Shared.Filters;

public class BadRequestFilter : IEndpointFilter
{
    public string Result { get; private set; }

    public async ValueTask<object> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
		try
		{
			var result = await next(ctx);

			// These cases here are for Idempotency checking: So we can tidy up the responses and make them consistent.

			return (ctx.HttpContext.Response, Result: result) switch 
			{
                { Response.StatusCode: 409, Response.HasStarted: false, Result: string detail } => 
					Results.Problem(detail, statusCode: 409),

                { Response.StatusCode: 409, Response.HasStarted: false } => 
					Results.Conflict(),

                { Response.StatusCode: 400, Response.HasStarted: false, Result: string detail } =>
                    Results.ValidationProblem(new Dictionary<string, string[]> { ["Summary"] = [detail] }),

                { Response.StatusCode: > 400 and < 500, Response.HasStarted: false, Result: string detail } =>
					Results.Problem(detail, statusCode: ctx.HttpContext.Response.StatusCode),

                { Response.StatusCode: > 400 and < 500, Response.HasStarted: false } => 
					Results.Problem(statusCode: ctx.HttpContext.Response.StatusCode),

                _ => result
			};

		}
		catch (ConflictException ex)
		{
            return Results.Problem(ex.Message, statusCode: 409);
        }
		catch (Exception ex) when(ex is ArgumentException or 
										ArgumentNullException or 
										BadHttpRequestException or
										BadRequestException)
		{
			return Results.ValidationProblem(new Dictionary<string, string[]> { ["Summary"] = [ex.Message] });
		}
    }
}