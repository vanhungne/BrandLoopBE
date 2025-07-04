﻿    using System;
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
            [StringLength(32)]
            public string UID { get; set; }

            [StringLength(100)]
            public string? Nickname { get; set; }

            public string Bio { get; set; }

            [StringLength(100)]
            public string ContentCategory { get; set; }

            [StringLength(255)]
            public string Location { get; set; }

            [StringLength(255)]
            public string Languages { get; set; }

            [StringLength(255)]
            public string? PortfolioUrl { get; set; }

            public decimal? AverageRate { get; set; }

            public bool Verified { get; set; } = false;

            [StringLength(255)]
            public string? Facebook { get; set; }

            [StringLength(255)]
            public string? Instagram { get; set; }

            [StringLength(255)]
            public string? Tiktok { get; set; }

            [StringLength(255)]
            public string? Youtube { get; set; }

            public int? FollowerCount { get; set; }

            public decimal? EngagementRate { get; set; }

            [StringLength(10)]
            public string Gender { get; set; }

            public DateOnly? DayOfBirth { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public DateTime UpdatedAt { get; set; } = DateTime.Now;

            public int? InfluencerTypeId { get; set; }

            public bool IsPriorityListed { get; set; } = false;        // ưu tiên search KOC // gói 1
            public bool IsFeaturedOnHome { get; set; } = false;        // hiển thị "đề xuất" trên home 24h // gói 2
            public bool HasExclusiveBanner { get; set; } = false;      // banner độc quyền 3 ngày // gói 3
            public bool IsInSpotlight { get; set; } = false;           // spotlight chuyên môn // gói 4

        // Navigation properties
        [ForeignKey("InfluencerTypeId")]
            public InfluencerType InfluencerType { get; set; }
            
            [ForeignKey("UID")]
            public virtual User User { get; set; }
        }
    }
