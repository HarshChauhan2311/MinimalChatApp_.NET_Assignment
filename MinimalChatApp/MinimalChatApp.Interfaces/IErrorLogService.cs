namespace MinimalChatApp.Interfaces
{
    public interface IErrorLogService
    {
        Task LogAsync(Exception ex);
    }
}
