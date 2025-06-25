using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.ChatDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrandLoop.Application.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
                await _chatService.UpdateOnlineStatusAsync(userId, true, Context.ConnectionId);

                // Notify all users about this user coming online
                await Clients.Others.SendAsync("UserOnline", new
                {
                    userId = userId,
                    timestamp = DateTime.UtcNow
                });

                // Send current online users list to the newly connected user
                var onlineUsers = await _chatService.GetOnlineUsersAsync(userId);
                await Clients.Caller.SendAsync("OnlineUsersList", onlineUsers);

                // Send updated online count to all users
                var onlineCount = await _chatService.GetOnlineUsersCountAsync();
                await Clients.All.SendAsync("OnlineUsersCount", onlineCount);

                // AUTO-LOAD: Send recent chats and unread count when user connects
                await GetRecentChats();
                await GetUnreadCount();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await _chatService.UpdateOnlineStatusAsync(userId, false);

                // Notify all users about this user going offline
                await Clients.Others.SendAsync("UserOffline", new
                {
                    userId = userId,
                    timestamp = DateTime.UtcNow
                });

                // Send updated online count to all users
                var onlineCount = await _chatService.GetOnlineUsersCountAsync();
                await Clients.All.SendAsync("OnlineUsersCount", onlineCount);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // Get current online users list
        public async Task GetOnlineUsers()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            var onlineUsers = await _chatService.GetOnlineUsersAsync(userId);
            await Clients.Caller.SendAsync("OnlineUsersList", onlineUsers);
        }

        // Get recent chats with online status
        public async Task GetRecentChats()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            var recentChats = await _chatService.GetRecentChatsWithOnlineStatusAsync(userId);
            await Clients.Caller.SendAsync("RecentChatsList", recentChats);
        }

        // Get chat history
        public async Task GetChatHistory(string contactId, int page = 1, int pageSize = 50)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                await Clients.Caller.SendAsync("ChatHistoryError", new { error = "User not authenticated" });
                return;
            }

            try
            {
                var messages = await _chatService.GetChatHistoryAsync(userId, contactId, page, pageSize);

                // Transform messages to include necessary information
                var chatHistory = messages.Select(m => new
                {
                    messageId = m.MessageId,
                    senderId = m.SenderId,
                    receiverId = m.ReceiverId,
                    senderName = m.Sender?.FullName,
                    receiverName = m.Receiver?.FullName,
                    content = m.Content,
                    messageType = m.MessageType.ToString(),
                    replyToMessageId = m.ReplyToMessageId,
                    replyToMessage = m.ReplyToMessage != null ? new
                    {
                        messageId = m.ReplyToMessage.MessageId,
                        content = m.ReplyToMessage.Content,
                        senderName = m.ReplyToMessage.Sender?.FullName
                    } : null,
                    createdAt = m.CreatedAt,
                    attachmentUrl = m.AttachmentUrl,
                    status = m.Status.ToString(),
                    // Check if message is read by current user (if they are the receiver)
                    isRead = m.ReceiverId == userId ?
                        (bool?)(m.ReadStatuses?.Any(rs => rs.UserId == userId && rs.ReadAt != null) == true) :
                        null,
                    // Check if message is delivered
                    isDelivered = m.ReceiverId == userId ?
                        (bool?)(m.ReadStatuses?.Any(rs => rs.UserId == userId && rs.DeliveredAt != null) == true) :
                        null
                }).ToList();

                await Clients.Caller.SendAsync("ChatHistoryReceived", new
                {
                    contactId = contactId,
                    page = page,
                    pageSize = pageSize,
                    messages = chatHistory,
                    hasMore = messages.Count() == pageSize // Indicates if there might be more messages
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ChatHistoryError", new { error = ex.Message });
            }
        }

        // Get unread messages count
        public async Task GetUnreadCount()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                var count = await _chatService.GetUnreadCountAsync(userId);
                await Clients.Caller.SendAsync("UnreadCountReceived", new { count });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UnreadCountError", new { error = ex.Message });
            }
        }

        // Send message to specific user
        public async Task SendMessage(string receiverId, string content, string messageType = "Text", int? replyToMessageId = null)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                return;

            try
            {
                // Parse message type
                Enum.TryParse<MessageType>(messageType, out var msgType);

                // Save message to database
                var message = await _chatService.SendMessageAsync(senderId, receiverId, content, msgType, replyToMessageId);

                // Send to receiver
                var receiverConnections = await _chatService.GetUserConnectionsAsync(receiverId);
                var connectionList = receiverConnections.ToList();
                if (receiverConnections.Any())
                {
                    await Clients.Clients(connectionList).SendAsync("ReceiveMessage", new
                    {
                        messageId = message.MessageId,
                        senderId = message.SenderId,
                        senderName = message.Sender?.FullName,
                        content = message.Content,
                        messageType = message.MessageType.ToString(),
                        replyToMessageId = message.ReplyToMessageId,
                        createdAt = message.CreatedAt,
                        attachmentUrl = message.AttachmentUrl
                    });

                    // Mark as delivered
                    await _chatService.MarkAsDeliveredAsync(message.MessageId, receiverId);
                }

                // Send confirmation back to sender
                await Clients.Caller.SendAsync("MessageSent", new
                {
                    messageId = message.MessageId,
                    status = "Sent",
                    createdAt = message.CreatedAt
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("MessageError", new { error = ex.Message });
            }
        }

        // Mark message as read
        public async Task MarkAsRead(int messageId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            var success = await _chatService.MarkAsReadAsync(messageId, userId);
            if (success)
            {
                await Clients.Caller.SendAsync("MessageRead", new { messageId });
            }
        }

        // Mark multiple messages as read
        public async Task MarkMultipleAsRead(int[] messageIds)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                var successCount = 0;
                foreach (var messageId in messageIds)
                {
                    var success = await _chatService.MarkAsReadAsync(messageId, userId);
                    if (success) successCount++;
                }

                await Clients.Caller.SendAsync("MultipleMessagesRead", new
                {
                    messageIds = messageIds,
                    successCount = successCount
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("MarkAsReadError", new { error = ex.Message });
            }
        }

        // Typing indicator
        public async Task StartTyping(string receiverId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                return;

            var receiverConnections = await _chatService.GetUserConnectionsAsync(receiverId);
            var connectionList = receiverConnections.ToList();
            await Clients.Clients(connectionList).SendAsync("UserStartTyping", senderId);
        }

        public async Task StopTyping(string receiverId)
        {
            var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId))
                return;

            var receiverConnections = await _chatService.GetUserConnectionsAsync(receiverId);
            var connectionList = receiverConnections.ToList();
            await Clients.Clients(connectionList).SendAsync("UserStopTyping", senderId);
        }

        // IMPROVED: Join private chat room with auto-loading chat history
        public async Task JoinChatRoom(string otherUserId, bool loadHistory = true, int pageSize = 20)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            var roomName = string.Compare(userId, otherUserId) < 0
                ? $"chat_{userId}_{otherUserId}"
                : $"chat_{otherUserId}_{userId}";

            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            // AUTO-LOAD: Get chat history when joining room
            if (loadHistory)
            {
                await GetChatHistory(otherUserId, 1, pageSize);

                // Mark all unread messages from this user as read
                await MarkChatAsRead(otherUserId);
            }

            // Send join confirmation
            await Clients.Caller.SendAsync("ChatRoomJoined", new
            {
                roomName = roomName,
                otherUserId = otherUserId,
                historyLoaded = loadHistory
            });
        }

        public async Task LeaveChatRoom(string otherUserId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            var roomName = string.Compare(userId, otherUserId) < 0
                ? $"chat_{userId}_{otherUserId}"
                : $"chat_{otherUserId}_{userId}";

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            await Clients.Caller.SendAsync("ChatRoomLeft", new
            {
                roomName = roomName,
                otherUserId = otherUserId
            });
        }

        // NEW: Mark all messages in a chat as read
        public async Task MarkChatAsRead(string otherUserId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                var success = await _chatService.MarkChatAsReadAsync(userId, otherUserId);
                if (success)
                {
                    await Clients.Caller.SendAsync("ChatMarkedAsRead", new { otherUserId });

                    // Update unread count
                    await GetUnreadCount();
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("MarkChatAsReadError", new { error = ex.Message });
            }
        }

        // NEW: Reconnect and restore chat state
        public async Task RestoreChatState(string currentChatUserId = null)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                // Load recent chats
                await GetRecentChats();

                // Load unread count
                await GetUnreadCount();

                // If user was in a specific chat, rejoin and load history
                if (!string.IsNullOrEmpty(currentChatUserId))
                {
                    await JoinChatRoom(currentChatUserId, true, 30); // Load more messages on restore
                }

                await Clients.Caller.SendAsync("ChatStateRestored", new
                {
                    currentChatUserId = currentChatUserId,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("RestoreChatStateError", new { error = ex.Message });
            }
        }

        // Delete message
        public async Task DeleteMessage(int messageId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                var success = await _chatService.DeleteMessageAsync(messageId, userId);
                if (success)
                {
                    await Clients.Caller.SendAsync("MessageDeleted", new { messageId });
                }
                else
                {
                    await Clients.Caller.SendAsync("DeleteMessageError", new { error = "Failed to delete message" });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("DeleteMessageError", new { error = ex.Message });
            }
        }

        // Get user online status
        public async Task GetUserOnlineStatus(string targetUserId)
        {
            try
            {
                var status = await _chatService.GetOnlineStatusAsync(targetUserId);
                await Clients.Caller.SendAsync("UserOnlineStatusReceived", new
                {
                    userId = targetUserId,
                    isOnline = status?.IsOnline ?? false,
                    lastSeen = status?.LastSeen,
                    deviceType = status?.DeviceType
                });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("UserOnlineStatusError", new { error = ex.Message });
            }
        }
    }
}