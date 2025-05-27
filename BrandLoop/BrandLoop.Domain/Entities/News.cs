using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Domain.Entities
{
    public class News
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NewsId { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Slug { get; set; }

        public string Content { get; set; }

        [StringLength(32)]
        public string Author { get; set; }

        [StringLength(255)]
        public string AuthorName { get; set; }

        public DateTime? PublishedAt { get; set; }

        [StringLength(20)]
        public NewsStatus Status { get; set; } = NewsStatus.Draft;

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
