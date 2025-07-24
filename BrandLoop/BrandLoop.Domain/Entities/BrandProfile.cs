using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class BrandProfile
    {
        [Key]
        public int BrandId { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; }

        [Required]
        [StringLength(255)]
        public string CompanyName { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(255)]
        public string? Logo { get; set; }

        [StringLength(50)]
        public string CompanySize { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }


        [StringLength(50)]
        public string? TaxCode { get; set; }

        public int? EstablishedYear { get; set; }

        [StringLength(255)]
        public string? Facebook { get; set; }

        [StringLength(255)]
        public string? Instagram { get; set; }

        [StringLength(255)]
        public string? Tiktok { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UID")]
        public virtual User User { get; set; }

        public virtual ICollection<Campaign> Campaigns { get; set; }
    }
}
