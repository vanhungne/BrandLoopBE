using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Models.News;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface INewsService
    {
        Task CreateNewsAsync(CreateNews news, IFormFile newsImage, string UID);
        Task<List<News>> GetAllNewsAsync(int pageNumber, int pageSize);
        Task<List<News>> GetMyNewsAsync(string uid, int pageNumber, int pageSize);
        Task<News> GetNewsByIdAsync(int newsId);
        Task<List<News>> SearchNewsAsync(string searchTerm, int pageNumber, int pageSize);
        Task<News> GetNewsBySlugAsync(string slug);
        Task<List<News>> GetNewsByCategoryAsync(string category, int pageNumber, int pageSize);
        Task<News> UpdateNewsAsync(UpdateNews news, IFormFile newsImage,string UID);
        Task DeleteNewsAsync(int newsId, string UID);
        Task<List<News>> GetAllPendingNewsAsync(int pageNumber, int pageSize);
        Task RejectNewsAsync(int newsId);
        Task ApproveNewsAsync(int newsId);
    }
}
