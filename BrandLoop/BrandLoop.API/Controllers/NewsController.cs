using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.Authen;
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
        public async Task<ActionResult<List<News>>> GetAllNews([FromQuery] PaginationFilter filter)
        {
            try
            {
                var newsList = await _newsService.GetAllNewsAsync();
                var totalRecords = newsList.Count;
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any news"));

                var pagedData = newsList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<News>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<News>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get news: {ex}"));
            }
        }

        [HttpGet("my-news")]
        public async Task<ActionResult<List<News>>> GetMyNews([FromQuery] PaginationFilter filter)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var myNews = await _newsService.GetMyNewsAsync(uid);
                var totalRecords = myNews.Count;
                if (myNews == null || !myNews.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No news found for this user"));

                var pagedData = myNews
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
                var response = new PaginationResponse<News>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<News>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<News>>> SearchNews([FromQuery]string searchTerm,[FromQuery] PaginationFilter filter)
        {
            try
            {
                var newsList = await _newsService.SearchNewsAsync(searchTerm);
                var totalRecords = newsList.Count;
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No news found for this search term"));

                var pagedData = newsList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<News>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<News>>.SuccessResult(response));
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
        public async Task<ActionResult<List<News>>> GetNewsByCategory([FromQuery]string category, [FromQuery] PaginationFilter filter)
        {
            try
            {
                var newsList = await _newsService.GetNewsByCategoryAsync(category);
                var totalRecords = newsList.Count;
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No news found for this category"));

                var pagedData = newsList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<News>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<News>>.SuccessResult(response));
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
        public async Task<ActionResult<List<News>>> GetAllPendingNews([FromQuery] PaginationFilter filter)
        {
            try
            {
                var pendingNews = await _newsService.GetAllPendingNewsAsync();
                var totalRecords = pendingNews.Count;
                if (pendingNews == null || !pendingNews.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("No pending news found"));

                var pagedData = pendingNews
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<News>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<News>>.SuccessResult(response));
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
