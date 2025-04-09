using Microsoft.AspNetCore.SignalR;

namespace MinimalChatApp.Providers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // You can use "sub", "email", or any other claim depending on your JWT setup
            return connection.User?.FindFirst("email")?.Value;
        }
    }
}
