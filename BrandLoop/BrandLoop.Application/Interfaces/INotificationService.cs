using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(string userId, string notificationType, string content, NotificationTarget target = NotificationTarget.All, int? relatedId = null);
        Task<List<Notification>> GetUserNotificationsAsync(string userId, int pageSize = 20, int pageNumber = 1);
        Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<List<Notification>> GetAdminNotificationsAsync(int pageSize = 20, int pageNumber = 1);
        Task<List<Notification>> GetUnreadAdminNotificationsAsync();

        // Realtime notification methods
        Task SendNotificationToUserAsync(string userId, string message, string type = "info");
        Task SendNotificationToAdminAsync(string message, string type = "info");
        Task SendNotificationToAllAsync(string message, string type = "info");
        Task SendNotificationToGroupAsync(string groupName, string message, string type = "info");
    }
}
