using BrandLoop.Domain.Entities;
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
        Task<List<News>> GetMyNews(string uid, int pageNumber, int pageSize);
        Task<List<News>> GetsAllNews(int pageNumber, int pageSize);
        Task<News> GetNewsById(int newsId);
        Task<List<News>> SearchNews(string searchTerm, int pageNumber, int pageSize);
        Task<News> GetNewsBySlug(string slug);
        Task<List<News>> GetNewsByCategory(string category, int pageNumber, int pageSize);
        Task<News> UpdateNews(UpdateNews news, IFormFile newsImage);
        Task DeleteNews(int newsId);
        Task<List<News>> GetAllPendingNews(int pageNumber, int pageSize);
        Task RejectNews(int newsId);
        Task ApproveNews(int newsId);
    }
}
