using BrandLoop.Application.Hubs;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task<Notification> CreateNotificationAsync(string userId, string notificationType, string content, NotificationTarget target = NotificationTarget.All, int? relatedId = null)
        {
            var notification = new Notification
            {
                UID = userId,
                NotificationType = notificationType,
                Content = content,
                Target = target,
                RelatedId = relatedId,
                Status = NotificationStatus.Unread,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            var createdNotification = await _notificationRepository.CreateAsync(notification);

            // Send realtime notification
            await SendRealtimeNotification(createdNotification);

            return createdNotification;
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId, int pageSize = 20, int pageNumber = 1)
        {
            return await _notificationRepository.GetByUserIdAsync(userId, pageSize, pageNumber);
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _notificationRepository.GetUnreadByUserIdAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            return await _notificationRepository.DeleteAsync(notificationId);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _notificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<List<Notification>> GetAdminNotificationsAsync(int pageSize = 20, int pageNumber = 1)
        {
            return await _notificationRepository.GetAdminNotificationsAsync(pageSize, pageNumber);
        }

        public async Task<List<Notification>> GetUnreadAdminNotificationsAsync()
        {
            return await _notificationRepository.GetUnreadAdminNotificationsAsync();
        }

        // Realtime notification methods
        public async Task SendNotificationToUserAsync(string userId, string message, string type = "info")
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            });
        }

        public async Task SendNotificationToAdminAsync(string message, string type = "info")
        {
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            });
        }

        public async Task SendNotificationToAllAsync(string message, string type = "info")
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            });
        }

        public async Task SendNotificationToGroupAsync(string groupName, string message, string type = "info")
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", new
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            });
        }

        private async Task SendRealtimeNotification(Notification notification)
        {
            var notificationData = new
            {
                Id = notification.NotificationId,
                Type = notification.NotificationType,
                Content = notification.Content,
                Status = notification.Status.ToString(),
                Target = notification.Target.ToString(),
                CreatedAt = notification.CreatedAt,
                RelatedId = notification.RelatedId
            };

            switch (notification.Target)
            {
                case NotificationTarget.All:
                    await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationData);
                    break;
                case NotificationTarget.Influencer:
                    await _hubContext.Clients.Group("Influencer").SendAsync("ReceiveNotification", notificationData);
                    break;
                case NotificationTarget.Brand:
                    await _hubContext.Clients.Group("Brand").SendAsync("ReceiveNotification", notificationData);
                    break;
            }

            // Send to specific user
            if (!string.IsNullOrEmpty(notification.UID))
            {
                await _hubContext.Clients.User(notification.UID).SendAsync("ReceiveNotification", notificationData);
            }

            // Send to Admin group
            await _hubContext.Clients.Group("Admin").SendAsync("ReceiveAdminNotification", notificationData);
        }
    }
}