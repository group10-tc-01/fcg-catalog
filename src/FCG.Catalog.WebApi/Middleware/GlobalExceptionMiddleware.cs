using FCG.Catalog.Domain.Exception;
using FCG.Catalog.WebApi.Models;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace FCG.Catalog.WebApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            List<string> errors = new List<string>();

            switch (exception)
            {
                case BaseException be:
                    statusCode = be.StatusCode;
                    errors.Add(be.Message);
                    break;
                case ValidationException ve:
                    statusCode = HttpStatusCode.BadRequest;
                    errors.AddRange(ve.Errors.Select(e => e.ErrorMessage));
                    break;
                default:
                    errors.Add("An unexpected error occurred.");
                    break;
            }

            var apiResponse = ApiResponse<object>.ErrorResponse(errors, statusCode);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(apiResponse, options);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(payload);
        }
    }
}
