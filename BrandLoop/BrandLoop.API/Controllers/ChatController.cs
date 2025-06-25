using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.ChatDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrandLoop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var senderId = GetCurrentUserId();
                if (string.IsNullOrEmpty(senderId))
                    return Unauthorized();

                var message = await _chatService.SendMessageAsync(
                    senderId,
                    request.ReceiverId,
                    request.Content,
                    request.MessageType,
                    request.ReplyToMessageId
                );

                return Ok(new
                {
                    success = true,
                    messageId = message.MessageId,
                    createdAt = message.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("history/{contactId}")]
        public async Task<IActionResult> GetChatHistory(string contactId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var messages = await _chatService.GetChatHistoryAsync(userId, contactId, page, pageSize);
                return Ok(new { success = true, data = messages });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentChats()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var chats = await _chatService.GetRecentChatsWithOnlineStatusAsync(userId);
                return Ok(new { success = true, data = chats });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        // NEW: Get online users list
        [HttpGet("online-users")]
        public async Task<IActionResult> GetOnlineUsers()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var onlineUsers = await _chatService.GetOnlineUsersAsync(userId);
                return Ok(new { success = true, data = onlineUsers });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // NEW: Get online users count
        [HttpGet("online-users/count")]
        public async Task<IActionResult> GetOnlineUsersCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _chatService.GetOnlineUsersCountAsync(userId);
                return Ok(new { success = true, count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var count = await _chatService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        [HttpPost("mark-as-read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _chatService.MarkAsReadAsync(messageId, userId);
            return Ok(new { success });
        }

        [HttpDelete("message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _chatService.DeleteMessageAsync(messageId, userId);
            return Ok(new { success });
        }

        [HttpGet("user-status/{userId}")]
        public async Task<IActionResult> GetUserOnlineStatus(string userId)
        {
            var status = await _chatService.GetOnlineStatusAsync(userId);
            return Ok(status);
        }
    }


    // DTOs
    public class SendMessageRequest
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public int? ReplyToMessageId { get; set; }
    }
}
