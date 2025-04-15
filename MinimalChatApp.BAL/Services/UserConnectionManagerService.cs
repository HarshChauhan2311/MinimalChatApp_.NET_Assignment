using Microsoft.EntityFrameworkCore;
using MinimalChatApp.BAL.IServices;

namespace MinimalChatApp.BAL.Services
{ 
    public class UserConnectionManagerService : IUserConnectionManagerService
    {
        private static readonly Dictionary<string, string> _connections = new();

        public void AddConnection(string userId, string connectionId)
        {
            _connections[userId] = connectionId;
        }

        public void RemoveConnection(string userId)
        {
            _connections.Remove(userId);
        }

        public string? GetConnectionId(string userId)
        {
            _connections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }
    }


}
