using FoodOrder.Api.Common.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FoodOrder.Api.Common.Exceptions
{
    public sealed class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex,
                    "Not found error for {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
                await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex,
                    "Domain validation error for {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
                await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);
                await WriteProblem(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteProblem(HttpContext ctx, HttpStatusCode status, string detail)
        {
            ctx.Response.ContentType = "application/problem+json";
            ctx.Response.StatusCode = (int)status;

            var problem = new ProblemDetails
            {
                Status = (int)status,
                Title = status switch
                {
                    HttpStatusCode.NotFound => "Not Found",
                    HttpStatusCode.BadRequest => "Bad Request",
                    _ => "Server Error"
                },
                Detail = detail
            };

            await ctx.Response.WriteAsJsonAsync(problem);
        }
    }

}

