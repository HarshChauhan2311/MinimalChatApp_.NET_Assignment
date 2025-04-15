using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MinimalChatApp.DAL.Data;
using MinimalChatApp.Entity;

namespace MinimalChatApp.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RequestLoggingMiddleware
    {
        #region Private Variables
        private readonly RequestDelegate _next;
        #endregion

        #region Constructors 
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        #endregion

        public async Task Invoke(HttpContext context, AppDbContext db)
        {
            try
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

                // Use asynchronous database add
                await db.RequestLogs.AddAsync(log);
                await db.SaveChangesAsync();

                await _next(context);
            }
            catch (Exception ex)
            {
                throw;
            }

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
