using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.ChatDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BrandLoop.Application.Service
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<Message> SendMessageAsync(string senderId, string receiverId, string content,
            MessageType messageType = MessageType.Text, int? replyToMessageId = null)
        {
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId) || string.IsNullOrEmpty(content))
                throw new ArgumentException("SenderId, ReceiverId, and Content are required");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                MessageType = messageType,
                ReplyToMessageId = replyToMessageId,
                Status = MessageStatus.Sent,
                CreatedAt = DateTime.UtcNow
            };

            return await _chatRepository.SendMessageAsync(message);
        }

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(string userId1, string userId2, int page = 1, int pageSize = 50)
        {
            if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
                throw new ArgumentException("Both userIds are required");

            return await _chatRepository.GetChatHistoryAsync(userId1, userId2, page, pageSize);
        }

        public async Task<bool> MarkAsDeliveredAsync(int messageId, string userId)
        {
            try
            {
                await _chatRepository.UpdateReadStatusAsync(messageId, userId, deliveredAt: DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> MarkAsReadAsync(int messageId, string userId)
        {
            try
            {
                await _chatRepository.UpdateReadStatusAsync(messageId, userId, readAt: DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return 0;

            return await _chatRepository.GetUnreadCountAsync(userId);
        }

        public async Task<IEnumerable<dynamic>> GetRecentChatsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<dynamic>();

            return await _chatRepository.GetRecentChatsAsync(userId);
        }

        public async Task<bool> DeleteMessageAsync(int messageId, string userId)
        {
            return await _chatRepository.DeleteMessageAsync(messageId, userId);
        }

        public async Task UpdateOnlineStatusAsync(string userId, bool isOnline, string connectionId = null, string deviceType = "Web")
        {
            var status = new UserOnlineStatus
            {
                UserId = userId,
                IsOnline = isOnline,
                LastSeen = DateTime.UtcNow,
                ConnectionId = connectionId,
                DeviceType = deviceType
            };

            await _chatRepository.UpdateOnlineStatusAsync(status);
        }

        public async Task<UserOnlineStatus> GetOnlineStatusAsync(string userId)
        {
            return await _chatRepository.GetOnlineStatusAsync(userId);
        }

        public async Task<IEnumerable<string>> GetUserConnectionsAsync(string userId)
        {
            return await _chatRepository.GetUserConnectionsAsync(userId);
        }
        public async Task<IEnumerable<dynamic>> GetOnlineUsersAsync(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
                return new List<dynamic>();

            return await _chatRepository.GetOnlineUsersAsync(currentUserId);
        }

        public async Task<IEnumerable<dynamic>> GetRecentChatsWithOnlineStatusAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<dynamic>();

            return await _chatRepository.GetRecentChatsWithOnlineStatusAsync(userId);
        }

        public async Task<int> GetOnlineUsersCountAsync(string currentUserId = null)
        {
            return await _chatRepository.GetOnlineUsersCountAsync(currentUserId);
        }

        public Task<bool> MarkChatAsReadAsync(string userId, string otherUserId)
        {
            return _chatRepository.MarkChatAsReadAsync(userId, otherUserId);
        }
    }
}

