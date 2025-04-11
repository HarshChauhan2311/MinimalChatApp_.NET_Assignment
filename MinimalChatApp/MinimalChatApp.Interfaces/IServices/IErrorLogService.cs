namespace MinimalChatApp.Interfaces.IServices
{
    /// <summary>
    /// Service for logging application exceptions.
    /// </summary>
    public interface IErrorLogService
    {
        /// <summary>
        /// Logs an exception asynchronously.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        Task LogAsync(Exception ex);
    }

}
