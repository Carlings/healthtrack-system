using FluentValidation;
using HealthTrack.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HealthTrack.Api.Infrastructure
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            httpContext.Response.ContentType = "application/problem+json";

            switch (exception)
            {
                case ValidationException validationException:
                    await WriteValidationProblemDetailsAsync(httpContext, validationException, cancellationToken);
                    return true;

                case NotFoundException notFoundException:
                    await WriteProblemDetailsAsync(
                        httpContext,
                        HttpStatusCode.NotFound,
                        "Resource not found",
                        notFoundException.Message,
                        cancellationToken);
                    return true;

                case ForbiddenAccessException forbiddenAccessException:
                    await WriteProblemDetailsAsync(
                        httpContext,
                        HttpStatusCode.Forbidden,
                        "Forbidden",
                        forbiddenAccessException.Message,
                        cancellationToken);
                    return true;

                case UnauthorizedAccessException unauthorizedAccessException:
                    await WriteProblemDetailsAsync(
                        httpContext,
                        HttpStatusCode.Unauthorized,
                        "Unauthorized",
                        unauthorizedAccessException.Message,
                        cancellationToken);
                    return true;

                default:
                    await WriteProblemDetailsAsync(
                        httpContext,
                        HttpStatusCode.InternalServerError,
                        "Server error",
                        "An unexpected error occurred.",
                        cancellationToken);
                    return true;
            }
        }

        private static async Task WriteValidationProblemDetailsAsync(
            HttpContext context,
            ValidationException exception,
            CancellationToken cancellationToken)
        {
            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).Distinct().ToArray());

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-400-bad-request",
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        private static async Task WriteProblemDetailsAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string detail,
            CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }
    }
}
