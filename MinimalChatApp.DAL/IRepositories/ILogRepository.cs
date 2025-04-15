using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.IRepositories
{
    /// <summary>
    /// Defines methods for retrieving API request logs.
    /// </summary>
    public interface ILogRepository
    {
        /// <summary>
        /// Retrieves request logs within the specified time range.
        /// </summary>
        /// <param name="startTime">The start time of the log range.</param>
        /// <param name="endTime">The end time of the log range.</param>
        /// <returns>A list of request logs within the specified time range.</returns>
        Task<List<RequestLog>> GetLogsAsync(DateTime startTime, DateTime endTime);
    }
}
