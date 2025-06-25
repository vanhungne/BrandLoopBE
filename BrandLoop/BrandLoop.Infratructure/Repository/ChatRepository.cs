using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BrandLoop.Infratructure.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly BLDBContext _context;

        public ChatRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Create read status for receiver
            var readStatus = new MessageReadStatus
            {
                MessageId = message.MessageId,
                UserId = message.ReceiverId,
                DeliveredAt = DateTime.UtcNow
            };

            _context.MessageReadStatuses.Add(readStatus);
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(string userId1, string userId2, int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .Where(m => m.DeletedAt == null)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.ReplyToMessage)
                .Include(m => m.ReadStatuses)
                .OrderByDescending(m => m.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.MessageId == messageId);
        }

        public async Task<MessageReadStatus> UpdateReadStatusAsync(int messageId, string userId, DateTime? deliveredAt = null, DateTime? readAt = null)
        {
            var readStatus = await _context.MessageReadStatuses
                .FirstOrDefaultAsync(rs => rs.MessageId == messageId && rs.UserId == userId);

            if (readStatus == null)
            {
                readStatus = new MessageReadStatus
                {
                    MessageId = messageId,
                    UserId = userId
                };
                _context.MessageReadStatuses.Add(readStatus);
            }

            if (deliveredAt.HasValue)
                readStatus.DeliveredAt = deliveredAt.Value;

            if (readAt.HasValue)
                readStatus.ReadAt = readAt.Value;

            await _context.SaveChangesAsync();
            return readStatus;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .Where(m => !_context.MessageReadStatuses
                    .Any(rs => rs.MessageId == m.MessageId && rs.UserId == userId && rs.ReadAt != null))
                .CountAsync();
        }

        public async Task<IEnumerable<dynamic>> GetRecentChatsAsync(string userId)
        {
            // Bước 1: Lấy tất cả tin nhắn liên quan tới user
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.DeletedAt == null)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            // Bước 2: Nhóm theo đối phương (người đối thoại)
            var grouped = messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g =>
                {
                    var lastMessage = g.OrderByDescending(m => m.CreatedAt).First();
                    var unreadCount = g.Count(m =>
                        m.ReceiverId == userId &&
                        !_context.MessageReadStatuses.Any(rs =>
                            rs.MessageId == m.MessageId &&
                            rs.UserId == userId &&
                            rs.ReadAt != null));

                    return new
                    {
                        ContactId = g.Key,
                        LastMessage = lastMessage,
                        UnreadCount = unreadCount
                    };
                })
                .OrderByDescending(x => x.LastMessage.CreatedAt)
                .ToList();

            // Bước 3: Lấy thông tin người dùng từ ContactId
            var contactIds = grouped.Select(g => g.ContactId).ToList();
            var contacts = await _context.Users
                .Where(u => contactIds.Contains(u.UID))
                .Select(u => new { u.UID, u.FullName, u.ProfileImage })
                .ToListAsync();

            // Bước 4: Ghép thông tin để trả về
            var results = grouped.Select(chat => new
            {
                chat.ContactId,
                Contact = contacts.FirstOrDefault(c => c.UID == chat.ContactId),
                chat.LastMessage.Content,
                chat.LastMessage.CreatedAt,
                chat.UnreadCount,
                IsOnline = _context.UserOnlineStatuses.Any(s => s.UserId == chat.ContactId && s.IsOnline)
            });

            return results;
        }


        public async Task<bool> DeleteMessageAsync(int messageId, string userId)
        {
            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId && m.SenderId == userId);

            if (message == null) return false;

            message.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserOnlineStatus> UpdateOnlineStatusAsync(UserOnlineStatus status)
        {
            var existingStatus = await _context.UserOnlineStatuses
                .FirstOrDefaultAsync(s => s.UserId == status.UserId);

            if (existingStatus == null)
            {
                _context.UserOnlineStatuses.Add(status);
            }
            else
            {
                existingStatus.IsOnline = status.IsOnline;
                existingStatus.LastSeen = status.LastSeen;
                existingStatus.ConnectionId = status.ConnectionId;
                existingStatus.DeviceType = status.DeviceType;
            }

            await _context.SaveChangesAsync();
            return existingStatus ?? status;
        }

        public async Task<UserOnlineStatus> GetOnlineStatusAsync(string userId)
        {
            return await _context.UserOnlineStatuses
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<string>> GetUserConnectionsAsync(string userId)
        {
            return await _context.UserOnlineStatuses
                .Where(s => s.UserId == userId && s.IsOnline && !string.IsNullOrEmpty(s.ConnectionId))
                .Select(s => s.ConnectionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetOnlineUsersAsync(string currentUserId)
        {
            // Get all online users except current user
            var onlineUsers = await _context.UserOnlineStatuses
                .Where(s => s.IsOnline && s.UserId != currentUserId)
                .Include(s => s.User)
                .Select(s => new
                {
                    UserId = s.UserId,
                    FullName = s.User.FullName,
                    ProfileImage = s.User.ProfileImage,
                    LastSeen = s.LastSeen,
                    DeviceType = s.DeviceType,
                    IsOnline = s.IsOnline
                })
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return onlineUsers;
        }

        public async Task<IEnumerable<dynamic>> GetRecentChatsWithOnlineStatusAsync(string userId)
        {
            // Bước 1: Lấy tất cả tin nhắn liên quan tới user
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.DeletedAt == null)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            // Bước 2: Nhóm theo đối phương (người đối thoại)
            var grouped = messages
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g =>
                {
                    var lastMessage = g.OrderByDescending(m => m.CreatedAt).First();
                    var unreadCount = g.Count(m =>
                        m.ReceiverId == userId &&
                        !_context.MessageReadStatuses.Any(rs =>
                            rs.MessageId == m.MessageId &&
                            rs.UserId == userId &&
                            rs.ReadAt != null));

                    return new
                    {
                        ContactId = g.Key,
                        LastMessage = lastMessage,
                        UnreadCount = unreadCount
                    };
                })
                .OrderByDescending(x => x.LastMessage.CreatedAt)
                .ToList();

            // Bước 3: Lấy thông tin người dùng và trạng thái online từ ContactId
            var contactIds = grouped.Select(g => g.ContactId).ToList();
            var contactsWithStatus = await (from user in _context.Users
                                            where contactIds.Contains(user.UID)
                                            join status in _context.UserOnlineStatuses
                                                on user.UID equals status.UserId into statusGroup
                                            from status in statusGroup.DefaultIfEmpty()
                                            select new
                                            {
                                                user.UID,
                                                user.FullName,
                                                user.ProfileImage,
                                                IsOnline = status != null && status.IsOnline,
                                                LastSeen = status != null ? status.LastSeen : (DateTime?)null,
                                                DeviceType = status != null ? status.DeviceType : null
                                            }).ToListAsync();

            // Bước 4: Ghép thông tin để trả về
            var results = grouped.Select(chat =>
            {
                var contact = contactsWithStatus.FirstOrDefault(c => c.UID == chat.ContactId);
                return new
                {
                    chat.ContactId,
                    Contact = new
                    {
                        contact?.UID,
                        contact?.FullName,
                        contact?.ProfileImage,
                        contact?.IsOnline,
                        contact?.LastSeen,
                        contact?.DeviceType
                    },
                    chat.LastMessage.Content,
                    chat.LastMessage.CreatedAt,
                    chat.UnreadCount,
                    IsOnline = contact?.IsOnline ?? false
                };
            });

            return results;
        }

        public async Task<int> GetOnlineUsersCountAsync(string currentUserId = null)
        {
            var query = _context.UserOnlineStatuses.Where(s => s.IsOnline);

            if (!string.IsNullOrEmpty(currentUserId))
            {
                query = query.Where(s => s.UserId != currentUserId);
            }

            return await query.CountAsync();
        }
        public async Task<bool> MarkChatAsReadAsync(string userId, string otherUserId)
        {
            try
            {
                // Get all unread messages from otherUserId to userId
                var unreadMessages = await _context.Messages
                    .Where(m => m.SenderId == otherUserId &&
                               m.ReceiverId == userId &&
                               !m.ReadStatuses.Any(rs => rs.UserId == userId && rs.ReadAt != null))
                    .ToListAsync();

                foreach (var message in unreadMessages)
                {
                    var readStatus = await _context.MessageReadStatuses
                        .FirstOrDefaultAsync(rs => rs.MessageId == message.MessageId && rs.UserId == userId);

                    if (readStatus != null)
                    {
                        readStatus.ReadAt = DateTime.UtcNow;
                    }
                    else
                    {
                        _context.MessageReadStatuses.Add(new MessageReadStatus
                        {
                            MessageId = message.MessageId,
                            UserId = userId,
                            DeliveredAt = DateTime.UtcNow,
                            ReadAt = DateTime.UtcNow
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> MarkAsReadAsync(int messageId, string userId)
        {
            try
            {
                var readStatus = await _context.MessageReadStatuses
                    .FirstOrDefaultAsync(rs => rs.MessageId == messageId && rs.UserId == userId);

                if (readStatus != null)
                {
                    readStatus.ReadAt = DateTime.UtcNow;
                }
                else
                {
                    _context.MessageReadStatuses.Add(new MessageReadStatus
                    {
                        MessageId = messageId,
                        UserId = userId,
                        DeliveredAt = DateTime.UtcNow,
                        ReadAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> MarkAsDeliveredAsync(int messageId, string userId)
        {
            try
            {
                var readStatus = await _context.MessageReadStatuses
                    .FirstOrDefaultAsync(rs => rs.MessageId == messageId && rs.UserId == userId);

                if (readStatus != null)
                {
                    if (readStatus.DeliveredAt == null)
                    {
                        readStatus.DeliveredAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    _context.MessageReadStatuses.Add(new MessageReadStatus
                    {
                        MessageId = messageId,
                        UserId = userId,
                        DeliveredAt = DateTime.UtcNow
                    });
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


   
        // Method to get unread messages count by conversation
        public async Task<Dictionary<string, int>> GetUnreadCountByConversationAsync(string userId)
        {
            try
            {
                var unreadCounts = await _context.Messages
                    .Where(m => m.ReceiverId == userId &&
                               !m.ReadStatuses.Any(rs => rs.UserId == userId && rs.ReadAt != null))
                    .GroupBy(m => m.SenderId)
                    .Select(g => new { SenderId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.SenderId, x => x.Count);

                return unreadCounts;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, int>();
            }
        }

        // Method to get recent chats with unread count
        public async Task<IEnumerable<object>> GetRecentChatsWithUnreadCountAsync(string userId)
        {
            try
            {
                var recentMessages = await _context.Messages
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                    .Select(g => new
                    {
                        ContactId = g.Key,
                        LastMessage = g.OrderByDescending(m => m.CreatedAt).First(),
                        UnreadCount = g.Count(m => m.ReceiverId == userId &&
                                                 !m.ReadStatuses.Any(rs => rs.UserId == userId && rs.ReadAt != null))
                    })
                    .OrderByDescending(x => x.LastMessage.CreatedAt)
                    .ToListAsync();

                var result = new List<object>();
                foreach (var chat in recentMessages)
                {
                    var contact = await _context.Users.FindAsync(chat.ContactId);
                    var onlineStatus = await GetOnlineStatusAsync(chat.ContactId);

                    result.Add(new
                    {
                        contactId = chat.ContactId,
                        contactName = contact?.FullName,
                        lastMessage = new
                        {
                            content = chat.LastMessage.Content,
                            createdAt = chat.LastMessage.CreatedAt,
                            messageType = chat.LastMessage.MessageType.ToString(),
                            senderId = chat.LastMessage.SenderId
                        },
                        unreadCount = chat.UnreadCount,
                        isOnline = onlineStatus?.IsOnline ?? false,
                        lastSeen = onlineStatus?.LastSeen
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                return new List<object>();
            }
        }
    }
}