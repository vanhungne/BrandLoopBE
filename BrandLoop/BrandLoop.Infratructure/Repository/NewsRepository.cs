using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
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
                Status = Domain.Enums.NewsStatus.Published,
                CreatedAt = DateTimeHelper.GetVietnamNow(),
                UpdatedAt = DateTimeHelper.GetVietnamNow()
            };
            await _context.News.AddAsync(newNews);  
            await _context.SaveChangesAsync();
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

        public async Task<List<News>> GetAllPendingNews()
        {
            var pendingNews = await _context.News
                .Where(n => n.Status == Domain.Enums.NewsStatus.Draft)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return pendingNews;
        }

        public async Task<List<News>> GetNewsByCategory(string category)
        {
            var newsByCategory = await _context.News
                .Where(n => n.Category == category && n.Status == Domain.Enums.NewsStatus.Published)
                .OrderByDescending(n => n.CreatedAt)
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
                .FirstOrDefaultAsync(n => n.Slug == slug);
            return news;
        }

        public async Task<List<News>> GetsAllNews()
        {
            var allNews = await _context.News
                .Where(n => n.Status != Domain.Enums.NewsStatus.Deleted)
                .OrderByDescending(n => n.CreatedAt)
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

        public async Task<List<News>> SearchNews(string searchTerm)
        {
            var searchResults = await _context.News
                .Where(n => n.Title.Contains(searchTerm) || n.AuthorName.Contains(searchTerm))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return searchResults;
        }

        public async Task<News> UpdateNews(UpdateNews news, IFormFile newsImage)
        {
            var existingNews = await _context.News.FirstOrDefaultAsync(n => n.NewsId == news.NewsId);
            if (existingNews == null)
                throw new Exception("News not found");

            // Chỉ cập nhật FeaturedImage khi có newsImage mới
            if (newsImage != null)
            {
                var cloudinaryService = new CloundinaryRepository(_configuration);
                string newsImageUrl = await cloudinaryService.UploadImage(newsImage);
                existingNews.FeaturedImage = newsImageUrl;
            }

            // Cập nhật các trường khác
            existingNews.Title = news.Title;
            existingNews.Slug = GenerateSlug(news.Title);
            existingNews.Content = news.Content;
            existingNews.Category = news.Category;
            existingNews.UpdatedAt = DateTimeHelper.GetVietnamNow();

            _context.News.Update(existingNews);
            await _context.SaveChangesAsync();
            return existingNews;
        }

        public Task<List<News>> GetMyNews(string uid)
        {
            var myNews = _context.News
                .Where(n => n.Author == uid)
                .OrderByDescending(n => n.CreatedAt)
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
        public async Task<List<string>> GetAllSlugs()
        {
            var slugs = await _context.News
                .Select(n => n.Slug)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
            return slugs;
        }

        public async Task<List<string>> GetAllCategories()
        {
            var categories = await _context.News
                .Select(n => n.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            return categories;
        }

        public async Task<List<News>> GetsAllNewsPublished()
        {
            var allNews = await _context.News
                         .Where(n => n.Status == Domain.Enums.NewsStatus.Published)
                         .OrderByDescending(n => n.CreatedAt)
                         .ToListAsync();
                                return allNews;
        }
        public async Task UpdateNewsStatus(int newsId, NewsStatus status)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.NewsId == newsId);
            if (news == null)
                throw new Exception("News not found");

            news.Status = status;

            // Set PublishedAt when status is Published
            if (status == Domain.Enums.NewsStatus.Published)
            {
                news.PublishedAt = DateTimeHelper.GetVietnamNow();
            }
            else if (status == Domain.Enums.NewsStatus.Draft)
            {
                news.PublishedAt = null; // Clear publish date for draft
            }

            news.UpdatedAt = DateTimeHelper.GetVietnamNow();
            _context.News.Update(news);
            await _context.SaveChangesAsync();
        }

    }
}
