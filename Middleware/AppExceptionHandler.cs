using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OrderAndPaymentAPI.Exceptions;

namespace OrderAndPaymentAPI.Middleware;

public sealed class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not AppException appException)
        {
            return false;
        }

        httpContext.Response.StatusCode = appException.statusCode;

        var problem = new ProblemDetails
        {
            Status = appException.statusCode,
            Title = "Không thể thực hiện yêu cầu",
            Detail = appException.Message,
            Instance = httpContext.Request.Path.Value
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        await httpContext.Response.WriteAsJsonAsync(
            problem, cancellationToken: cancellationToken);

        return true;
    }
}
