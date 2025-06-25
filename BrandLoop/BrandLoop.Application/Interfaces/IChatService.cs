using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.ChatDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IChatService
    {
        Task<Message> SendMessageAsync(string senderId, string receiverId, string content, MessageType messageType = MessageType.Text, int? replyToMessageId = null);
        Task<IEnumerable<Message>> GetChatHistoryAsync(string userId1, string userId2, int page = 1, int pageSize = 50);
        Task<bool> MarkAsDeliveredAsync(int messageId, string userId);
        Task<bool> MarkAsReadAsync(int messageId, string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<dynamic>> GetRecentChatsAsync(string userId);
        Task<bool> DeleteMessageAsync(int messageId, string userId);
        Task UpdateOnlineStatusAsync(string userId, bool isOnline, string connectionId = null, string deviceType = "Web");
        Task<UserOnlineStatus> GetOnlineStatusAsync(string userId);
        Task<IEnumerable<string>> GetUserConnectionsAsync(string userId);

        Task<IEnumerable<dynamic>> GetOnlineUsersAsync(string currentUserId);
        Task<IEnumerable<dynamic>> GetRecentChatsWithOnlineStatusAsync(string userId);
        Task<int> GetOnlineUsersCountAsync(string currentUserId = null);
        Task<bool> MarkChatAsReadAsync(string userId, string otherUserId);
        Task<Message> GetMessageByIdAsync(int messageId);
        }
}
