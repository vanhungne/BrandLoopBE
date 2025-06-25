using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Banner
    {
        [Key]
        public int BannerId { get; set; }

        [Required, StringLength(32)]
        public int InfluenceId { get; set; }              // Links to InfluenceProfile

        [Required, StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(500)]
        public string TargetUrl { get; set; }       // e.g. link to profile or campaign page

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("InfluenceId")]
        public virtual InfluenceProfile InfluenceProfile { get; set; }
    }
}
