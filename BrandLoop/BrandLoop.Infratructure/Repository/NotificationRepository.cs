using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrandLoop.Infratructure.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly BLDBContext _context;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(BLDBContext context, ILogger<NotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateNotification(Notification notification)
        {
            try
            {
                notification.CreatedAt = DateTimeHelper.GetVietnamNow();
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return false;
            }
        }

        public async Task<List<Notification>> GetUserNotifications(string email, bool unreadOnly = false)
        {
            var query = _context.Notifications.Where(n => n.Email == email);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<bool> MarkAllNotificationsAsRead(string email)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.Email == email && !n.IsRead)
                    .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.Status = NotificationStatus.Read;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return false;
            }
        
        }

        public async Task<bool> MarkNotificationAsRead(int notificationId)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(notificationId);

                if (notification == null)
                    return false;

                notification.IsRead = true;
                notification.Status = NotificationStatus.Read;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return false;
            }
        }
    }
}
