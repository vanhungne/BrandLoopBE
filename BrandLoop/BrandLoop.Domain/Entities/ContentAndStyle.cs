using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class ContentAndStyle
    {
        [Key]
        public int ContentId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(200)]
        public string ContentName { get; set; }

        [StringLength(400)]
        public string VideoUrl { get; set; }

        [StringLength(400)]
        public string ImageUrl { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}
