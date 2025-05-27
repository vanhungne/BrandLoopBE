﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.News
{
    public class NewsBase
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string AuthorName { get; set; }
        public string FeaturedImage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
