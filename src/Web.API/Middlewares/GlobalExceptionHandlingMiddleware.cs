using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            ProblemDetails problem = new ProblemDetails()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "Server error",
                Title = "Server error",
                Detail = "An internal server error occurred."
            };

            string json = JsonSerializer.Serialize(problem);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
    }
}