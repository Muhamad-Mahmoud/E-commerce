using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;

namespace ECommerce.API.Middleware;

public sealed class RequestLoggingMiddleware
{
    private const int MaxBodyLengthToLog = 4096;

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var originalResponseBody = context.Response.Body;

        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await LogRequestAsync(context);

            await _next(context);

            stopwatch.Stop();

            await LogResponseAsync(context, stopwatch.ElapsedMilliseconds);

            // rewind stream before copying
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalResponseBody);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "HTTP {Method} {Path} failed after {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    // Request Logging
    private async Task LogRequestAsync(HttpContext context)
    {
        var request = context.Request;

        var userName = context.User?.Identity?.IsAuthenticated == true
            ? context.User.Identity!.Name ?? "Unknown"
            : "Anonymous";

        _logger.LogInformation(
            "HTTP {Method} {Url} started | User: {User} | IP: {IP}",
            request.Method,
            request.GetDisplayUrl(),
            userName,
            context.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

        if (IsBodyLoggable(request))
        {
            var body = await ReadRequestBodyAsync(request);
            if (!string.IsNullOrWhiteSpace(body))
            {
                _logger.LogDebug("Request Body: {Body}", MaskSensitiveData(body));
            }
        }
    }

    // Response Logging
    private async Task LogResponseAsync(HttpContext context, long elapsedMs)
    {
        var response = context.Response;

        var logLevel =
            response.StatusCode >= 500 ? LogLevel.Error :
            response.StatusCode >= 400 ? LogLevel.Warning :
            LogLevel.Information;

        string? responseBodyText = null;

        if (IsBodyLoggable(response))
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(
                context.Response.Body,
                Encoding.UTF8,
                leaveOpen: true);

            responseBodyText = await reader.ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
        }

        _logger.Log(
            logLevel,
            "HTTP {Method} {Path} completed | Status: {StatusCode} | Time: {ElapsedMs} ms | ContentType: {ContentType} | BodyLength: {BodyLength}",
            context.Request.Method,
            context.Request.Path,
            response.StatusCode,
            elapsedMs,
            response.ContentType ?? "none",
            response.ContentLength ?? responseBodyText?.Length ?? 0);
    }

    // Helpers
    private static bool IsBodyLoggable(HttpRequest request) =>
        request.ContentLength > 0 &&
        request.ContentLength <= MaxBodyLengthToLog &&
        request.ContentType?.Contains("application/json") == true;

    private static bool IsBodyLoggable(HttpResponse response) =>
        response.ContentType?.Contains("application/json") == true;

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return body;
    }

    private static string MaskSensitiveData(string body)
    {
        return body
            .Replace("password", "******", StringComparison.OrdinalIgnoreCase)
            .Replace("token", "******", StringComparison.OrdinalIgnoreCase);
    }
}
