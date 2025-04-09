using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.Interfaces.IRepositories;
using System.Collections.Concurrent;

namespace MinimalChatApp.Hubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        // Store active connections
        private static Dictionary<string, string> _userConnections = new();

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
                _userConnections[userName] = Context.ConnectionId;

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User.Identity?.Name;
            if (!string.IsNullOrEmpty(userName) && _userConnections.ContainsKey(userName))
                _userConnections.Remove(userName);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string toUser, string message)
        {
            if (_userConnections.TryGetValue(toUser, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", toUser, Context.User.Identity?.Name, message);
            }
        }
    }

}
