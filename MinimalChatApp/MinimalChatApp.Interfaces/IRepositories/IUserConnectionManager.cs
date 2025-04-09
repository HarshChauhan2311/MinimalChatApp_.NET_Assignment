namespace MinimalChatApp.Interfaces.IRepositories
{
    public interface IUserConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId);
        string? GetConnectionId(string userId);
    }
}
