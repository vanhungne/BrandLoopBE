using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IChatRepository
    {
        Task<Message> SendMessageAsync(Message message);
        Task<IEnumerable<Message>> GetChatHistoryAsync(string userId1, string userId2, int page, int pageSize);
        Task<Message> GetMessageByIdAsync(int messageId);
        Task<MessageReadStatus> UpdateReadStatusAsync(int messageId, string userId, DateTime? deliveredAt = null, DateTime? readAt = null);
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<dynamic>> GetRecentChatsAsync(string userId);
        Task<bool> DeleteMessageAsync(int messageId, string userId);
        Task<UserOnlineStatus> UpdateOnlineStatusAsync(UserOnlineStatus status);
        Task<UserOnlineStatus> GetOnlineStatusAsync(string userId);
        Task<IEnumerable<string>> GetUserConnectionsAsync(string userId);

        Task<IEnumerable<dynamic>> GetOnlineUsersAsync(string currentUserId);
        Task<IEnumerable<dynamic>> GetRecentChatsWithOnlineStatusAsync(string userId);
        Task<int> GetOnlineUsersCountAsync(string currentUserId = null);
        Task<bool> MarkChatAsReadAsync(string userId, string otherUserId);
        }
    }
