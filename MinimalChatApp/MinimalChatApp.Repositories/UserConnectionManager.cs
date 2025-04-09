using MinimalChatApp.Interfaces.IRepositories;

namespace MinimalChatApp.Repositories
{
    public class UserConnectionManager : IUserConnectionManager
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
