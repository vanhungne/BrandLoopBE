using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Resource
    {
        [Key]
        public int ResourceId { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        public string Content { get; set; }

        [StringLength(255)]
        public string Url { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
