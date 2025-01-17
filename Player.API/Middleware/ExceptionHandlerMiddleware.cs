using System.Net;
using Player.Domain.Dtos;

namespace Player.API.Middleware;
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e, logger);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        logger.LogError(exception, "Handling exception = {Type}", exception.GetType().Name);

        var response = EmptyResult.UnknownError("Unhandled exception occurred");
        if (exception is BadHttpRequestException)
        {
            response = EmptyResult.InvalidRequest(exception.Message);
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
#if DEBUG
            response.AppendDetails(exception.Message);
#endif
        }
        return context.Response.WriteAsJsonAsync(response);
    }
}