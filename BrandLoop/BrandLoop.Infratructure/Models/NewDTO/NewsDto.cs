using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.NewDTO
{
    public class NewsDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string Category { get; set; }
        public string FeaturedImage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    public class NewsListDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string AuthorName { get; set; }
        public string Category { get; set; }
        public string FeaturedImage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    public class NewsDetailDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string Category { get; set; }
        public string FeaturedImage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public AuthorDto Author { get; set; }
    }

    public class AuthorDto
    {
        public string UID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }

    public class MyNewsDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Category { get; set; }
        public string FeaturedImage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    public class PendingNewsDto
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string AuthorName { get; set; }
        public string Category { get; set; }
        public string FeaturedImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
