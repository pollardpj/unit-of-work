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

            return CreateResponse(ctx.HttpContext.Response, result);
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

    private static object CreateResponse(HttpResponse response, object result)
    {
        if (!response.HasStarted && response.StatusCode is >= 400 and < 500)
        {
            return result switch
            {
                string detail when response.StatusCode == 400 => 
                    Results.ValidationProblem(new Dictionary<string, string[]> { ["Summary"] = [detail] }),
                    
                string detail => 
                    Results.Problem(detail, statusCode: response.StatusCode),
                    
                _ => 
                    Results.Problem(statusCode: response.StatusCode)
            };
        }

        return result;
    }
}