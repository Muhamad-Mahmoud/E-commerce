using System.Net;
using System.Text.Json;
using ECommerce.API.Errors;

namespace ECommerce.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred accessing {RequestPath}", context.Request.Path);

                context.Response.ContentType = "application/json";

                // Determine Status Code based on Exception Type
                context.Response.StatusCode = ex switch
                {
                    ArgumentException => (int)HttpStatusCode.BadRequest,        // 400
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,       // 404
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
                    _ => (int)HttpStatusCode.InternalServerError                // 500
                };

                var response = _env.IsDevelopment()
                    ? new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
                    : new ApiErrorResponse(context.Response.StatusCode, 
                        context.Response.StatusCode == 500 ? "Internal Server Error" : ex.Message, 
                        null);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
