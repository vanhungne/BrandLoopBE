using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.NewDTO;
using BrandLoop.Infratructure.Models.News;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrandLoop.Application.Service.NewsService;

namespace BrandLoop.Application.Interfaces
{
    public interface INewsService
    {
        Task CreateNewsAsync(CreateNews news, IFormFile newsImage, string UID);
        Task<List<NewsListDto>> GetAllNewsAsync();
        Task<List<MyNewsDto>> GetMyNewsAsync(string uid);
        Task<List<NewsListDto>> SearchNewsAsync(string searchTerm);
        Task<NewsDetailDto> GetNewsBySlugAsync(string slug);
        Task<List<NewsListDto>> GetNewsByCategoryAsync(string category);
        Task<NewsDetailDto> UpdateNewsAsync(UpdateNews news, IFormFile newsImage, string UID);
        Task DeleteNewsAsync(int newsId, string UID);
        Task<List<PendingNewsDto>> GetAllPendingNewsAsync();
        Task ApproveNewsAsync(int newsId);
        Task RejectNewsAsync(int newsId);
        Task<NewsDetailDto> GetNewsByIdAsync(int newsId);
        Task<List<SlugDto>> GetAllSlugsAsync();
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<List<NewsListDto>> GetsAllNewsPublished();
        Task UpdateNewsStatusAsync(int newsId, NewsStatus status, string uid);
    }
}
