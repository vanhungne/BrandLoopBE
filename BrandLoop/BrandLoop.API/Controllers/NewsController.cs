using BrandLoop.API.Models;
using BrandLoop.API.Response;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.Authen;
using BrandLoop.Infratructure.Models.NewDTO;
using BrandLoop.Infratructure.Models.News;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList.Extensions;
using static BrandLoop.Application.Service.NewsService;

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
        public async Task<ActionResult<List<NewsListDto>>> GetAllNews([FromQuery] PaginationFilter filter)
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

                var response = new PaginationResponse<NewsListDto>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<NewsListDto>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get news: {ex}"));
            }
        }
        [HttpGet("all_published")]
        public async Task<ActionResult<List<NewsListDto>>> GetAllNewsPublished([FromQuery] PaginationFilter filter)
        {
            try
            {
                var newsList = await _newsService.GetsAllNewsPublished();
                var totalRecords = newsList.Count;
                if (newsList == null || !newsList.Any())
                    return NotFound(ApiResponse<string>.ErrorResult("Can not found any news"));

                var pagedData = newsList
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

                var response = new PaginationResponse<NewsListDto>(
                pagedData,
                filter.PageNumber,
                filter.PageSize,
                totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<NewsListDto>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult($"Error while get news: {ex}"));
            }
        }

        [HttpGet("my-news")]
        public async Task<ActionResult<List<MyNewsDto>>> GetMyNews([FromQuery] PaginationFilter filter)
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
                var response = new PaginationResponse<MyNewsDto>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<MyNewsDto>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<NewsListDto>>> SearchNews([FromQuery] string searchTerm, [FromQuery] PaginationFilter filter)
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

                var response = new PaginationResponse<NewsListDto>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<NewsListDto>>.SuccessResult(response));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<NewsDetailDto>> GetNewsBySlug(string slug)
        {
            try
            {
                var news = await _newsService.GetNewsBySlugAsync(slug);
                if (news == null)
                    return NotFound(ApiResponse<string>.ErrorResult("News not found"));

                return Ok(ApiResponse<NewsDetailDto>.SuccessResult(news));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<NewsListDto>>> GetNewsByCategory([FromQuery] string category, [FromQuery] PaginationFilter filter)
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

                var response = new PaginationResponse<NewsListDto>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<NewsListDto>>.SuccessResult(response));
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
        public async Task<ActionResult<List<PendingNewsDto>>> GetAllPendingNews([FromQuery] PaginationFilter filter)
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

                var response = new PaginationResponse<PendingNewsDto>(
                    pagedData,
                    filter.PageNumber,
                    filter.PageSize,
                    totalRecords
                );
                return Ok(ApiResponse<PaginationResponse<PendingNewsDto>>.SuccessResult(response));
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
        [HttpGet("slugs")]
        public async Task<ActionResult<List<SlugDto>>> GetAllSlugs()
        {
            try
            {
                var slugs = await _newsService.GetAllSlugsAsync();
                return Ok(ApiResponse<List<SlugDto>>.SuccessResult(slugs));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var categories = await _newsService.GetAllCategoriesAsync();
                return Ok(ApiResponse<List<CategoryDto>>.SuccessResult(categories));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateNewsStatus([FromBody] UpdateNewsStatusRequest request)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _newsService.UpdateNewsStatusAsync(request.NewsId, request.Status, uid);

                string statusMessage = request.Status == NewsStatus.Published ? "published" : "drafted";
                return Ok(ApiResponse<string>.SuccessResult($"News status updated to {statusMessage} successfully"));
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

        // Alternative: Update status by route parameters
        [HttpPut("update-status/{newsId}/{status}")]
        public async Task<IActionResult> UpdateNewsStatus(int newsId, NewsStatus status)
        {
            try
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _newsService.UpdateNewsStatusAsync(newsId, status, uid);

                string statusMessage = status == NewsStatus.Published ? "published" : "drafted";
                return Ok(ApiResponse<string>.SuccessResult($"News status updated to {statusMessage} successfully"));
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
        [HttpGet("id/{newsId}")]
        public async Task<ActionResult<NewsDetailDto>> GetNewsById(int newsId)
        {
            try
            {
                var news = await _newsService.GetNewsByIdAsync(newsId);
                if (news == null)
                    return NotFound(ApiResponse<string>.ErrorResult("News not found"));

                return Ok(ApiResponse<NewsDetailDto>.SuccessResult(news));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.ErrorResult(ex.Message));
            }
        }
    }
}