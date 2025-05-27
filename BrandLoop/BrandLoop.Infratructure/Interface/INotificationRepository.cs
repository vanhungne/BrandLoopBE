using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;

namespace BrandLoop.Infratructure.Interface
{
    public interface INotificationRepository
    {
        Task<bool> CreateNotification(Notification notification);
        Task<List<Notification>> GetUserNotifications(string uid, bool unreadOnly = false);
        Task<bool> MarkNotificationAsRead(int notificationId);
        Task<bool> MarkAllNotificationsAsRead(string uid);
    }
}
