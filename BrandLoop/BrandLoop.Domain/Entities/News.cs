using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Slug { get; set; }

        public string Content { get; set; }

        [StringLength(255)]
        public string Author { get; set; }

        public DateTime? PublishedAt { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "draft";

        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(255)]
        public string FeaturedImage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("Author")]
        public virtual User AuthorUser { get; set; }
    }
}
