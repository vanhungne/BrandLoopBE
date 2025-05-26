using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    // 4. Influence Profile entity
    public class InfluenceProfile
    {
        [Key]
        public int InfluenceId { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Nickname { get; set; }

        public string Bio { get; set; }

        [StringLength(100)]
        public string ContentCategory { get; set; }

        [StringLength(255)]
        public string Location { get; set; }

        [StringLength(255)]
        public string Languages { get; set; }

        [StringLength(255)]
        public string PortfolioUrl { get; set; }

        public decimal? AverageRate { get; set; }

        public bool Verified { get; set; } = false;

        [StringLength(255)]
        public string Facebook { get; set; }

        [StringLength(255)]
        public string Instagram { get; set; }

        [StringLength(255)]
        public string Tiktok { get; set; }

        [StringLength(255)]
        public string Youtube { get; set; }

        public int? FollowerCount { get; set; }

        public decimal? EngagementRate { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        public DateOnly? DayOfBirth { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("Email")]
        public virtual User User { get; set; }
    }
}
