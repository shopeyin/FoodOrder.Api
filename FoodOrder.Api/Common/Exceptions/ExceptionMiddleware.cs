using FoodOrder.Api.Common.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodOrder.Api.Common.Exceptions
{
    public sealed class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (DomainException ex)
            {
                await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                // Don’t leak internals in prod
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
