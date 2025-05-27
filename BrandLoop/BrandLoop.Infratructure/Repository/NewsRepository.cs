using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.News;
using BrandLoop.Infratructure.Persistence;
using BrandLoop.Infratructure.Reporitory;
using BrandLoop.Shared.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly BLDBContext _context;
        private readonly IConfiguration _configuration;
        public NewsRepository(BLDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task ApproveNews(int newsId)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.NewsId == newsId);
            if (news == null)
                throw new Exception("News not found");

            news.Status = Domain.Enums.NewsStatus.Published;
            news.PublishedAt = DateTimeHelper.GetVietnamNow();
            _context.News.Update(news);
            _context.SaveChanges();
        }

        public async Task CreateNews(CreateNews news, IFormFile newsImage, string uid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UID == uid);
            if (user == null)
                throw new Exception("User not found");

            string newsImageUrl = null;
            if (newsImage != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                newsImageUrl = await cloudinaryService.UploadImage(newsImage);
            }

            var newNews = new News
            {
                Title = news.Title,
                Slug = GenerateSlug(news.Title),
                Content = news.Content,
                Author = uid,
                AuthorName = user.FullName,
                Category = news.Category,
                FeaturedImage = newsImageUrl,
                Status = Domain.Enums.NewsStatus.Draft,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                UpdatedAt = DateTimeHelper.GetVietnamNow()
            };
        }

        public async Task DeleteNews(int newsId)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.NewsId == newsId);
            if (news == null)
                throw new Exception("News not found");

            news.Status = Domain.Enums.NewsStatus.Deleted;
            _context.News.Update(news);
            _context.SaveChanges();
        }

        public async Task<List<News>> GetAllPendingNews(int pageNumber, int pageSize)
        {
            var pendingNews = await _context.News
                .Where(n => n.Status == Domain.Enums.NewsStatus.Draft)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return pendingNews;
        }

        public async Task<List<News>> GetNewsByCategory(string category, int pageNumber, int pageSize)
        {
            var newsByCategory = await _context.News
                .Where(n => n.Category == category && n.Status == Domain.Enums.NewsStatus.Published)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return newsByCategory;
        }

        public async Task<News> GetNewsById(int newsId)
        {
            var news = await _context.News
                .Include(n => n.AuthorUser)
                .FirstOrDefaultAsync(n => n.NewsId == newsId && n.Status != Domain.Enums.NewsStatus.Deleted);
            return news;
        }

        public async Task<News> GetNewsBySlug(string slug)
        {
            var news = await _context.News
                .Include(n => n.AuthorUser)
                .FirstOrDefaultAsync(n => n.Slug == slug && n.Status == Domain.Enums.NewsStatus.Published);
            return news;
        }

        public async Task<List<News>> GetsAllNews(int pageNumber, int pageSize)
        {
            var allNews = await _context.News
                .Where(n => n.Status != Domain.Enums.NewsStatus.Deleted)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return allNews;
        }

        public async Task RejectNews(int newsId)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.NewsId == newsId);
            if (news == null)
                throw new Exception("News not found");

            news.Status = Domain.Enums.NewsStatus.Rejected;
                _context.News.Update(news);
                _context.SaveChanges();
        }

        public async Task<List<News>> SearchNews(string searchTerm, int pageNumber, int pageSize)
        {
            var searchResults = await _context.News
                .Where(n => n.Title.Contains(searchTerm) || n.AuthorName.Contains(searchTerm))
                .OrderByDescending(n => n.CreatedAt)    
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return searchResults;
        }

        public async Task<News> UpdateNews(UpdateNews news, IFormFile newsImage)
        {
            var existingNews = await _context.News.FirstOrDefaultAsync(n => n.NewsId == news.NewsId);
            if (existingNews == null)
                throw new Exception("News not found");

            string newsImageUrl = null;
            if (newsImage != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                newsImageUrl = await cloudinaryService.UploadImage(newsImage);
            }

            existingNews.Title = news.Title;
            existingNews.Slug = GenerateSlug(news.Title);
            existingNews.Content = news.Content;
            existingNews.Category = news.Category;
            existingNews.FeaturedImage = newsImageUrl;
            existingNews.UpdatedAt = DateTimeHelper.GetVietnamNow();
            _context.News.Update(existingNews);
            await _context.SaveChangesAsync();
            return existingNews;
        }

        public Task<List<News>> GetMyNews(string uid, int pageNumber, int pageSize)
        {
            var myNews = _context.News
                .Where(n => n.Author == uid)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return myNews;
        }

        public static string GenerateSlug(string title)
        {
            string normalized = title.ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"\p{IsCombiningDiacriticalMarks}+", "")
                              .Normalize(NormalizationForm.FormC);
            normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
            normalized = Regex.Replace(normalized, @"\s+", "-").Trim('-');
            return normalized;
        }
    }
}
