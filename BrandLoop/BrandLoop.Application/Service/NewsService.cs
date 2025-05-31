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

        public async Task<List<News>> GetAllNewsAsync()
        {
            return await _newsRepository.GetsAllNews();
        }

        public async Task<List<News>> GetAllPendingNewsAsync()
        {
            return await _newsRepository.GetAllPendingNews();
        }

        public async Task<List<News>> GetMyNewsAsync(string uid)
        {
            return await _newsRepository.GetMyNews(uid);
        }

        public async Task<List<News>> GetNewsByCategoryAsync(string category)
        {
            return await _newsRepository.GetNewsByCategory(category);
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

        public async Task<List<News>> SearchNewsAsync(string searchTerm)
        {
            return await _newsRepository.SearchNews(searchTerm);
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
