namespace MinimalChatApp.BAL.IServices
{
    /// <summary>
    /// Manages user connections for real-time communication, such as with SignalR.
    /// </summary>
    public interface IUserConnectionManagerService
    {
        /// <summary>
        /// Adds a connection ID for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to associate with the connection.</param>
        /// <param name="connectionId">The unique connection ID to add.</param>
        void AddConnection(string userId, string connectionId);

        /// <summary>
        /// Removes the connection associated with the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose connection should be removed.</param>
        void RemoveConnection(string userId);

        /// <summary>
        /// Retrieves the connection ID associated with the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose connection ID is requested.</param>
        /// <returns>The connection ID if it exists; otherwise, null.</returns>
        string? GetConnectionId(string userId);
    }

}
