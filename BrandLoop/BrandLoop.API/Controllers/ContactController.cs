using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrandLoop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactMessageService _contactMessageService;

        public ContactController(IContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        [HttpPost]
        public async Task<ActionResult<ContactMessageResponseDto>> CreateMessage([FromBody] ContactMessageCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _contactMessageService.CreateMessageAsync(createDto);
                return CreatedAtAction(nameof(GetMessage), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi gửi tin nhắn", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactMessageResponseDto>>> GetAllMessages()
        {
            try
            {
                var messages = await _contactMessageService.GetAllMessagesAsync();
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách tin nhắn", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactMessageResponseDto>> GetMessage(int id)
        {
            try
            {
                var message = await _contactMessageService.GetMessageByIdAsync(id);
                if (message == null)
                    return NotFound(new { message = "Không tìm thấy tin nhắn" });

                return Ok(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy tin nhắn", error = ex.Message });
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<ActionResult<ContactMessageResponseDto>> MarkAsRead(int id)
        {
            try
            {
                var result = await _contactMessageService.MarkAsReadAsync(id);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy tin nhắn" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật tin nhắn", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            try
            {
                var result = await _contactMessageService.DeleteMessageAsync(id);
                if (!result)
                    return NotFound(new { message = "Không tìm thấy tin nhắn" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa tin nhắn", error = ex.Message });
            }
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var count = await _contactMessageService.GetUnreadCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy số tin nhắn chưa đọc", error = ex.Message });
            }
        }
    }
}
