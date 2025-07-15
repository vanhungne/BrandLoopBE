using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Models.News;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface INewsRepository
    {
        Task CreateNews(CreateNews news, IFormFile newsImage, string uid);
        Task<List<News>> GetMyNews(string uid);
        Task<List<News>> GetsAllNews();
        Task<List<News>> GetsAllNewsPublished();
        Task<News> GetNewsById(int newsId);
        Task<List<News>> SearchNews(string searchTerm);
        Task<News> GetNewsBySlug(string slug);
        Task<List<News>> GetNewsByCategory(string category);
        Task<News> UpdateNews(UpdateNews news, IFormFile newsImage);
        Task DeleteNews(int newsId);
        Task<List<News>> GetAllPendingNews();
        Task RejectNews(int newsId);
        Task ApproveNews(int newsId);
        Task<List<string>> GetAllSlugs();
        Task<List<string>> GetAllCategories();
        Task UpdateNewsStatus(int newsId, NewsStatus status);
    }
}
