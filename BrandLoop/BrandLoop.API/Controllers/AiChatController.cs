using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Models.ChatDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BrandLoop.API.Controllers
{
    [ApiController, Route("api/ai/[action]")]
    public class AiChatController : ControllerBase
    {
        private readonly IChatAiService _ai;
        private readonly ILogger<AiChatController> _logger;

        public AiChatController(IChatAiService ai, ILogger<AiChatController> logger)
        {
            _ai = ai;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request, CancellationToken ct)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.UserPrompt))
                {
                    return BadRequest("Prompt cannot be empty");
                }

                _logger.LogInformation("Processing chat request");

                var response = await _ai.AskAsync(request, ct);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access to AI service");
                return StatusCode(401, new { error = "API key configuration error", message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation in AI service");
                return StatusCode(400, new { error = "Invalid request", message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error in AI service");
                return StatusCode(502, new { error = "External service error", message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in chat endpoint");
                return StatusCode(500, new { error = "Internal server error", message = "An unexpected error occurred" });
            }
        }
    }
}