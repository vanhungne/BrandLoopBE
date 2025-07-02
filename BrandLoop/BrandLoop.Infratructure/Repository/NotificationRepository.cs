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

        public async Task<List<Notification>> GetUserNotifications(string uid, bool unreadOnly = false)
        {
            var query = _context.Notifications.Where(n => n.UID == uid);

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<bool> MarkAllNotificationsAsRead(string uid)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UID == uid && !n.IsRead)
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

        public async Task<Notification> CreateAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.Now;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> GetByIdAsync(int notificationId)
        {
            return await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        public async Task<List<Notification>> GetByUserIdAsync(string userId, int pageSize = 20, int pageNumber = 1)
        {
            return await _context.Notifications
                .Where(n => n.UID == userId && n.Status != NotificationStatus.Deleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UID == userId && n.Status == NotificationStatus.Unread)
                .OrderByDescending(n => n.CreatedAt)
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetByTargetAsync(NotificationTarget target, int pageSize = 20, int pageNumber = 1)
        {
            return await _context.Notifications
                .Where(n => n.Target == target && n.Status != NotificationStatus.Deleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Read;
                notification.IsRead = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UID == userId && n.Status == NotificationStatus.Unread)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.Status = NotificationStatus.Read;
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.Status = NotificationStatus.Deleted;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UID == userId && n.Status == NotificationStatus.Unread);
        }

        public async Task<List<Notification>> GetAdminNotificationsAsync(int pageSize = 20, int pageNumber = 1)
        {
            return await _context.Notifications
                .Where(n => n.Target == NotificationTarget.All && n.Status != NotificationStatus.Deleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadAdminNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => n.Target == NotificationTarget.All && n.Status == NotificationStatus.Unread)
                .OrderByDescending(n => n.CreatedAt)
                .Include(n => n.User)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
