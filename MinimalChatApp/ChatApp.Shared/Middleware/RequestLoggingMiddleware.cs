using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MinimalChatApp.ChatApp.Infrastructure.Data;
using MinimalChatApp.ChatApp.Shared.Models;

namespace MinimalChatApp.ChatApp.Shared.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, AppDbContext db)
        {
            var request = context.Request;
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var path = request.Path;

            // Get body
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            // Get username from JWT if exists
            string? username = context.User.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : null;

            var log = new RequestLog
            {
                IPAddress = ip,
                RequestBody = body,
                RequestPath = path,
                Username = username,
                Timestamp = DateTime.UtcNow
            };

            db.RequestLogs.Add(log);
            await db.SaveChangesAsync();

            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
