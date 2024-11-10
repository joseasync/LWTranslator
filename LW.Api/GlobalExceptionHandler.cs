using LW.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LW.Api
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is TranslationException translationException) {

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = translationException.Error,
                    Detail = translationException.Message,
                    Type = "Bad Request"
                };

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return await _problemDetailsService.TryWriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = problemDetails
                    });        
            }

            if (exception is ArgumentNullException argumentException)
            {

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Argument error",
                    Detail = argumentException.Message,
                    Type = "Bad Request"
                };

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return await _problemDetailsService.TryWriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = problemDetails
                    });
            }

            return true;
        }
    }
}