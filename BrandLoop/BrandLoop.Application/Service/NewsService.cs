using AutoMapper;
using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.Entities;
using BrandLoop.Domain.Enums;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Models.NewDTO;
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
        private readonly IMapper _mapper;

        public NewsService(INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
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
            if (news.Author != UID)
                throw new UnauthorizedAccessException("You do not have permission to delete this news.");
            await _newsRepository.DeleteNews(newsId);
        }

        public async Task<List<NewsListDto>> GetAllNewsAsync()
        {
            var newsList = await _newsRepository.GetsAllNews();
            return _mapper.Map<List<NewsListDto>>(newsList);
        }

        public async Task<List<PendingNewsDto>> GetAllPendingNewsAsync()
        {
            var pendingNews = await _newsRepository.GetAllPendingNews();
            return _mapper.Map<List<PendingNewsDto>>(pendingNews);
        }

        public async Task<List<MyNewsDto>> GetMyNewsAsync(string uid)
        {
            var myNews = await _newsRepository.GetMyNews(uid);
            return _mapper.Map<List<MyNewsDto>>(myNews);
        }

        public async Task<List<NewsListDto>> GetNewsByCategoryAsync(string category)
        {
            var newsByCategory = await _newsRepository.GetNewsByCategory(category);
            return _mapper.Map<List<NewsListDto>>(newsByCategory);
        }

        public async Task<NewsDetailDto> GetNewsByIdAsync(int newsId)
        {
            var news = await _newsRepository.GetNewsById(newsId);
            return _mapper.Map<NewsDetailDto>(news);
        }

        public async Task<NewsDetailDto> GetNewsBySlugAsync(string slug)
        {
            var news = await _newsRepository.GetNewsBySlug(slug);
            return _mapper.Map<NewsDetailDto>(news);
        }

        public async Task RejectNewsAsync(int newsId)
        {
            await _newsRepository.RejectNews(newsId);
        }

        public async Task<List<NewsListDto>> SearchNewsAsync(string searchTerm)
        {
            var searchResults = await _newsRepository.SearchNews(searchTerm);
            return _mapper.Map<List<NewsListDto>>(searchResults);
        }

        public async Task<News> UpdateNewsAsync(UpdateNews news, IFormFile newsImage, string UID)
        {
            var existNews = await _newsRepository.GetNewsById(news.NewsId);
            if (existNews.Author != UID)
                throw new UnauthorizedAccessException("You do not have permission to update this news.");
            return await _newsRepository.UpdateNews(news, newsImage);
        }
        public async Task<List<SlugDto>> GetAllSlugsAsync()
        {
            var slugs = await _newsRepository.GetAllSlugs();
            return slugs.Select(s => new SlugDto { Slug = s }).ToList();
        }
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _newsRepository.GetAllCategories();
            return categories.Select(c => new CategoryDto { Category = c }).ToList();
        }

        public async Task<List<NewsListDto>> GetsAllNewsPublished()
        {
            var newsList = await _newsRepository.GetsAllNewsPublished();
            return _mapper.Map<List<NewsListDto>>(newsList);
        }

        public async Task UpdateNewsStatusAsync(int newsId, NewsStatus status, string uid)
        {
            var news = await _newsRepository.GetNewsById(newsId);
            if (news == null)
                throw new Exception("News not found");

            if (news.Author != uid)
                throw new UnauthorizedAccessException("You do not have permission to update this news status.");

            await _newsRepository.UpdateNewsStatus(newsId, status);
        }

        // 5. Create DTO for the request
        public class UpdateNewsStatusRequest
        {
            public int NewsId { get; set; }
            public NewsStatus Status { get; set; }
        }

        public class SlugDto
        {
            public string Slug { get; set; }
        }

        public class CategoryDto
        {
            public string Category { get; set; }
        }
    }
}