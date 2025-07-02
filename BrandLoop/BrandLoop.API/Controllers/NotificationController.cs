using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(
                    request.UserId,
                    request.NotificationType,
                    request.Content,
                    request.Target,
                    request.RelatedId
                );

                return Ok(new { Success = true, Data = notification });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Users can only see their own notifications unless they're admin
                if (currentUserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var notifications = await _notificationService.GetUserNotificationsAsync(userId, pageSize, pageNumber);
                return Ok(new { Success = true, Data = notifications });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/unread")]
        public async Task<IActionResult> GetUnreadNotifications(string userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (currentUserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                return Ok(new { Success = true, Data = notifications });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(string userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (currentUserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(new { Success = true, Data = count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var result = await _notificationService.MarkAsReadAsync(notificationId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("user/{userId}/read-all")]
        public async Task<IActionResult> MarkAllAsRead(string userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (currentUserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var result = await _notificationService.MarkAllAsReadAsync(userId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                var result = await _notificationService.DeleteNotificationAsync(notificationId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // Admin specific endpoints
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminNotifications([FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
        {
            try
            {
                var notifications = await _notificationService.GetAdminNotificationsAsync(pageSize, pageNumber);
                return Ok(new { Success = true, Data = notifications });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet("admin/unread")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUnreadAdminNotifications()
        {
            try
            {
                var notifications = await _notificationService.GetUnreadAdminNotificationsAsync();
                return Ok(new { Success = true, Data = notifications });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        // Realtime notification endpoints
        [HttpPost("send-to-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotificationToUser([FromBody] SendNotificationRequest request)
        {
            try
            {
                await _notificationService.SendNotificationToUserAsync(request.UserId, request.Message, request.Type);
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("send-to-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotificationToAdmin([FromBody] SendAdminNotificationRequest request)
        {
            try
            {
                await _notificationService.SendNotificationToAdminAsync(request.Message, request.Type);
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("send-to-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotificationToAll([FromBody] SendBroadcastNotificationRequest request)
        {
            try
            {
                await _notificationService.SendNotificationToAllAsync(request.Message, request.Type);
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("send-to-group")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotificationToGroup([FromBody] SendGroupNotificationRequest request)
        {
            try
            {
                await _notificationService.SendNotificationToGroupAsync(request.GroupName, request.Message, request.Type);
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }

    // Request DTOs
    public class CreateNotificationRequest
    {
        public string UserId { get; set; }
        public string NotificationType { get; set; }
        public string Content { get; set; }
        public NotificationTarget Target { get; set; } = NotificationTarget.All;
        public int? RelatedId { get; set; }
    }

    public class SendNotificationRequest
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } = "info";
    }

    public class SendAdminNotificationRequest
    {
        public string Message { get; set; }
        public string Type { get; set; } = "info";
    }

    public class SendBroadcastNotificationRequest
    {
        public string Message { get; set; }
        public string Type { get; set; } = "info";
    }

    public class SendGroupNotificationRequest
    {
        public string GroupName { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } = "info";
    }
}