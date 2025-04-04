using MinimalChatApp.ChatApp.Shared.Models;

namespace MinimalChatApp.ChatApp.Core.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> SendMessageAsync(int senderId, int receiverId, string content);
    }
}
