using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            // Add user to appropriate groups based on role
            if (!string.IsNullOrEmpty(userRole))
            {
                switch (userRole.ToLower())
                {
                    case "Admin":
                        await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
                        break;
                    case "Influencer":
                        await Groups.AddToGroupAsync(Context.ConnectionId, "Influencer");
                        break;
                    case "Brand":
                        await Groups.AddToGroupAsync(Context.ConnectionId, "Brand");
                        break;
                }
            }

            // Send connection confirmation
            await Clients.Caller.SendAsync("Connected", new
            {
                Message = "Connected to notification hub",
                UserId = userId,
                Role = userRole,
                ConnectionId = Context.ConnectionId,
                Timestamp = DateTime.Now
            });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

            // Remove user from groups
            if (!string.IsNullOrEmpty(userRole))
            {
                switch (userRole.ToLower())
                {
                    case "Admin":
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admin");
                        break;
                    case "Influencer":
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Influencer");
                        break;
                    case "Brand":
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Brand");
                        break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Client can join specific notification groups
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedGroup", $"Joined group: {groupName}");
        }

        // Client can leave specific notification groups
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("LeftGroup", $"Left group: {groupName}");
        }

        // Mark notification as read
        public async Task MarkNotificationAsRead(int notificationId)
        {
            // This would typically call the notification service
            await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
        }

        // Send typing indicator (for chat features)
        public async Task SendTypingIndicator(string receiverUserId)
        {
            await Clients.User(receiverUserId).SendAsync("TypingIndicator", Context.UserIdentifier);
        }

        // Send message to specific user
        public async Task SendMessageToUser(string receiverUserId, string message)
        {
            await Clients.User(receiverUserId).SendAsync("ReceiveMessage", new
            {
                SenderId = Context.UserIdentifier,
                Message = message,
                Timestamp = DateTime.Now
            });
        }

        // Admin specific methods
        [Authorize(Roles = "Admin")]
        public async Task SendAdminBroadcast(string message)
        {
            await Clients.All.SendAsync("AdminBroadcast", new
            {
                Message = message,
                Timestamp = DateTime.Now,
                AdminId = Context.UserIdentifier
            });
        }

        [Authorize(Roles = "Admin")]
        public async Task SendToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("GroupMessage", new
            {
                Message = message,
                Group = groupName,
                Timestamp = DateTime.Now,
                AdminId = Context.UserIdentifier
            });
        }

        // Get user connection info
        public async Task GetConnectionInfo()
        {
            await Clients.Caller.SendAsync("ConnectionInfo", new
            {
                ConnectionId = Context.ConnectionId,
                UserId = Context.UserIdentifier,
                Role = Context.User?.FindFirst(ClaimTypes.Role)?.Value,
                Timestamp = DateTime.Now
            });
        }
    }
}
