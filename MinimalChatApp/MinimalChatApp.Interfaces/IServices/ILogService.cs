using Microsoft.AspNetCore.Mvc;

namespace MinimalChatApp.MinimalChatApp.Interfaces.IServices
{
    /// <summary>
    /// Service for retrieving logged API requests.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Retrieves logs within an optional time range.
        /// </summary>
        /// <param name="startTime">Optional start time to filter logs.</param>
        /// <param name="endTime">Optional end time to filter logs.</param>
        /// <returns>An <see cref="IActionResult"/> containing the filtered logs.</returns>
        Task<IActionResult> GetLogsAsync(DateTime? startTime, DateTime? endTime);
    }

}
