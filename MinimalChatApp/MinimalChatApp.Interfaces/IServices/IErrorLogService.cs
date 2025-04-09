namespace MinimalChatApp.Interfaces.IServices
{
    public interface IErrorLogService
    {
        Task LogAsync(Exception ex);
    }
}
