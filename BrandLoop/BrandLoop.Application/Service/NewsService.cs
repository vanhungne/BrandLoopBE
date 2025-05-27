using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.News;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task ApproveNewsAsync(int newsId)
        {
            await _newsRepository.ApproveNews(newsId);
        }

        public async Task CreateNewsAsync(CreateNews news, IFormFile newsImage, string UID)
        {
            await _newsRepository.CreateNews(news, newsImage, UID);
        }

        public async Task DeleteNewsAsync(int newsId, string UID)
        {
            var news = await _newsRepository.GetNewsById(newsId);
            if(news.Author != UID)
            throw new UnauthorizedAccessException("You do not have permission to delete this news.");

            await _newsRepository.DeleteNews(newsId);
        }

        public async Task<List<News>> GetAllNewsAsync(int pageNumber, int pageSize)
        {
            return await _newsRepository.GetsAllNews(pageNumber, pageSize);
        }

        public async Task<List<News>> GetAllPendingNewsAsync(int pageNumber, int pageSize)
        {
            return await _newsRepository.GetAllPendingNews(pageNumber, pageSize);
        }

        public async Task<List<News>> GetMyNewsAsync(string uid, int pageNumber, int pageSize)
        {
            return await _newsRepository.GetMyNews(uid, pageNumber, pageSize);
        }

        public async Task<List<News>> GetNewsByCategoryAsync(string category, int pageNumber, int pageSize)
        {
            return await _newsRepository.GetNewsByCategory(category, pageNumber, pageSize);
        }

        public async Task<News> GetNewsByIdAsync(int newsId)
        {
            return await _newsRepository.GetNewsById(newsId);
        }

        public async Task<News> GetNewsBySlugAsync(string slug)
        {
            return await _newsRepository.GetNewsBySlug(slug);
        }

        public async Task RejectNewsAsync(int newsId)
        {
            await _newsRepository.RejectNews(newsId);
        }

        public async Task<List<News>> SearchNewsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            return await _newsRepository.SearchNews(searchTerm, pageNumber, pageSize);
        }

        public async Task<News> UpdateNewsAsync(UpdateNews news, IFormFile newsImage, string UID)
        {
            var existNews = await _newsRepository.GetNewsById(news.NewsId);
            if (existNews.Author != UID)
                throw new UnauthorizedAccessException("You do not have permission to delete this news.");

            return await _newsRepository.UpdateNews(news, newsImage);
        }
    }
}
