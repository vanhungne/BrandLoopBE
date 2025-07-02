using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface INotificationRepository
    {
        Task<bool> CreateNotification(Notification notification);
        Task<List<Notification>> GetUserNotifications(string uid, bool unreadOnly = false);
        Task<bool> MarkNotificationAsRead(int notificationId);
        Task<bool> MarkAllNotificationsAsRead(string uid);

        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> GetByIdAsync(int notificationId);
        Task<List<Notification>> GetByUserIdAsync(string userId, int pageSize = 20, int pageNumber = 1);
        Task<List<Notification>> GetUnreadByUserIdAsync(string userId);
        Task<List<Notification>> GetByTargetAsync(NotificationTarget target, int pageSize = 20, int pageNumber = 1);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<bool> DeleteAsync(int notificationId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<List<Notification>> GetAdminNotificationsAsync(int pageSize = 20, int pageNumber = 1);
        Task<List<Notification>> GetUnreadAdminNotificationsAsync();
        Task<bool> UpdateAsync(Notification notification);
    }
}
