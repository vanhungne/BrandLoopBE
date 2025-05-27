using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace BrandLoop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateNews([FromForm] CreateNews news, IFormFile? newsImage)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _newsService.CreateNewsAsync(news, newsImage, uid);
                return Ok(ApiResponse<string>.SuccessResult("News created successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<News>>> GetAllNews(int pageNumber, int pageSize)
        {
            try
            {
                var newsList = await _newsService.GetAllNewsAsync(pageNumber, pageSize);
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any news"));

                return Ok(ApiResponse<List<News>>.SuccessResult(newsList));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get news: {ex}"));
            }
        }

        [HttpGet("my-news")]
        public async Task<ActionResult<List<News>>> GetMyNews(int pageNumber, int pageSize)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var myNews = await _newsService.GetMyNewsAsync(uid, pageNumber, pageSize);
                if (myNews == null || !myNews.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No news found for this user"));

                return Ok(ApiResponse<List<News>>.SuccessResult(myNews));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<News>>> SearchNews(string searchTerm, int pageNumber, int pageSize)
        {
            try
            {
                var newsList = await _newsService.SearchNewsAsync(searchTerm, pageNumber, pageSize);
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No news found for this search term"));

                return Ok(ApiResponse<List<News>>.SuccessResult(newsList));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<News>> GetNewsBySlug(string slug)
        {
            try
            {
                var news = await _newsService.GetNewsBySlugAsync(slug);
                if (news == null)
                    return NotFound(ApiResponse<string>.ErrorResult("News not found"));

                return Ok(ApiResponse<News>.SuccessResult(news));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<News>>> GetNewsByCategory(string category, int pageNumber, int pageSize)
        {
            try
            {
                var newsList = await _newsService.GetNewsByCategoryAsync(category, pageNumber, pageSize);
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No news found for this category"));

                return Ok(ApiResponse<List<News>>.SuccessResult(newsList));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpPut("update/{newsId}")]
        public async Task<IActionResult> UpdateNews([FromForm] UpdateNews news, IFormFile? newsImage)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedNews = await _newsService.UpdateNewsAsync(news, newsImage, uid);
                return Ok(ApiResponse<News>.SuccessResult(updatedNews));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpDelete("delete/{newsId}")]
        public async Task<IActionResult> DeleteNews(int newsId)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _newsService.DeleteNewsAsync(newsId, uid);
                return Ok(ApiResponse<string>.SuccessResult("News deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<string>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<ActionResult<List<News>>> GetAllPendingNews(int pageNumber, int pageSize)
        {
            try
            {
                var pendingNews = await _newsService.GetAllPendingNewsAsync(pageNumber, pageSize);
                if (pendingNews == null || !pendingNews.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No pending news found"));
                return Ok(ApiResponse<List<News>>.SuccessResult(pendingNews));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("approve/{newsId}")]
        public async Task<IActionResult> ApproveNews(int newsId)
        {
            try
            {
                await _newsService.ApproveNewsAsync(newsId);
                return Ok(ApiResponse<string>.SuccessResult("News approved successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reject/{newsId}")]
        public async Task<IActionResult> RejectNews(int newsId)
        {
            try
            {
                await _newsService.RejectNewsAsync(newsId);
                return Ok(ApiResponse<string>.SuccessResult("News rejected successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }
    }
}
