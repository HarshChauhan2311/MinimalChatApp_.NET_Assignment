using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApp.Interfaces.IRepositories;
using MinimalChatApp.Interfaces.IServices;
using MinimalChatApp.MinimalChatApp.Interfaces.IServices;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace MinimalChatApp.Hubs
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        // Store active connections - using ConcurrentDictionary for thread safety
        private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        private readonly IGroupService _groupService;
        private readonly IErrorLogService _errorLogService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(
            IGroupService groupService,
            IErrorLogService errorLogService,
            ILogger<ChatHub> logger)
        {
            _groupService = groupService;
            _errorLogService = errorLogService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userName = Context.User.Identity?.Name;
                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("Connection attempt without authentication");
                    Context.Abort();
                    return;
                }

                // Remove any existing connection for this user
                _userConnections.TryRemove(userName, out _);

                // Add the new connection
                if (_userConnections.TryAdd(userName, Context.ConnectionId))
                {
                    _logger.LogInformation($"User {userName} connected with connection ID: {Context.ConnectionId}");
                    await base.OnConnectedAsync();
                }
                else
                {
                    _logger.LogError($"Failed to add connection for user {userName}");
                    Context.Abort();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in OnConnectedAsync for connection {Context.ConnectionId}");
                await _errorLogService.LogAsync(ex);
                Context.Abort();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userName = Context.User.Identity?.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    if (_userConnections.TryRemove(userName, out var connectionId))
                    {
                        _logger.LogInformation($"User {userName} disconnected from connection {connectionId}");
                    }
                }
                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in OnDisconnectedAsync for connection {Context.ConnectionId}");
                await _errorLogService.LogAsync(ex);
            }
        }

        //public async Task SendMessage(string toUser, string message)
        //{
        //    try
        //    {
        //        var senderEmail = Context.User.Identity?.Name;
        //        if (string.IsNullOrEmpty(senderEmail))
        //        {
        //            _logger.LogWarning("Message attempt without authentication");
        //            throw new HubException("User not authenticated");
        //        }

        //        // Check if it's a group message (toUser starts with "group:")
        //        if (toUser.StartsWith("group:"))
        //        {
        //            var groupId = toUser.Substring(6); // Remove "group:" prefix
        //            var group = await _groupService.GetGroupByIdAsync(Convert.ToInt16(groupId));

        //            if (group == null)
        //            {
        //                _logger.LogWarning($"Group {groupId} not found");
        //                throw new HubException("Group not found");
        //            }

        //            // Verify sender is a member of the group
        //            if (!group.Members.Contains(senderEmail))
        //            {
        //                _logger.LogWarning($"User {senderEmail} attempted to send message to group {groupId} without being a member");
        //                throw new HubException("You are not a member of this group");
        //            }

        //            // Send message to all group members except sender
        //            var sentCount = 0;
        //            foreach (var memberEmail in group.Members.Where(m => m != senderEmail))
        //            {
        //                if (_userConnections.TryGetValue(memberEmail, out var connectionId))
        //                {
        //                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", memberEmail, senderEmail, message);
        //                    sentCount++;
        //                }
        //            }
        //            _logger.LogInformation($"Group message sent to {sentCount} members of group {groupId}");
        //        }
        //        else
        //        {
        //            // Direct message to a user
        //            if (_userConnections.TryGetValue(toUser, out var receiverConnectionId))
        //            {
        //                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", toUser, senderEmail, message);
        //                _logger.LogInformation($"Direct message sent from {senderEmail} to {toUser}");
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"Attempted to send message to offline user {toUser}");
        //                throw new HubException("Recipient is not connected");
        //            }
        //        }
        //    }
        //    catch (HubException hu)
        //    {
        //        _logger.LogWarning(hu, "Hub exception occurred");
        //        await _errorLogService.LogAsync(hu);
        //        throw; // Re-throw HubException as is
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error in SendMessage for connection {Context.ConnectionId}");
        //        await _errorLogService.LogAsync(ex);
        //        throw new HubException("An error occurred while sending the message");
        //    }
        //}



        public async Task SendMessage(string toUser, string message)
        {
            try
            {
                var senderEmail = Context.User.Identity?.Name;
                if (string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogWarning("Message attempt without authentication");
                    throw new HubException("User not authenticated");
                }

                // Check if it's a group message (toUser starts with "group:")
                if (toUser.StartsWith("group:"))
                {
                    await SendGroupMessage(toUser, message);
                }
                else
                {
                    await SendPersonalMessage(toUser, message);
                }
            }
            catch (HubException hu)
            {
                _logger.LogWarning(hu, "Hub exception occurred");
                await _errorLogService.LogAsync(hu);
                throw; // Re-throw HubException as is
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SendMessage for connection {Context.ConnectionId}");
                await _errorLogService.LogAsync(ex);
                throw new HubException("An error occurred while sending the message");
            }
        }

        private async Task SendPersonalMessage(string toUser, string message)
        {
            var senderEmail = Context.User.Identity?.Name;

            // Direct message to a user
            if (_userConnections.TryGetValue(toUser, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", toUser, senderEmail, message);
                _logger.LogInformation($"Direct message sent from {senderEmail} to {toUser}");
            }
            else
            {
                _logger.LogWarning($"Attempted to send message to offline user {toUser}");
                throw new HubException("Recipient is not connected");
            }
        }

        private async Task SendGroupMessage(string toUser, string message)
        {
            var senderEmail = Context.User.Identity?.Name;
            var groupId = toUser.Substring(6); // Remove "group:" prefix
            var group = await _groupService.GetGroupByIdAsync(Convert.ToInt16(groupId));

            if (group == null)
            {
                _logger.LogWarning($"Group {groupId} not found");
                throw new HubException("Group not found");
            }

            // Verify sender is a member of the group
            if (!group.Members.Contains(senderEmail))
            {
                _logger.LogWarning($"User {senderEmail} attempted to send message to group {groupId} without being a member");
                throw new HubException("You are not a member of this group");
            }

            // Send message to all group members except sender
            var sentCount = 0;
            foreach (var memberEmail in group.Members.Where(m => m != senderEmail))
            {
                if (_userConnections.TryGetValue(memberEmail, out var connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", toUser, senderEmail, message);
                    sentCount++;
                }
            }
            _logger.LogInformation($"Group message sent to {sentCount} members of group {groupId}");
        }
    }
}