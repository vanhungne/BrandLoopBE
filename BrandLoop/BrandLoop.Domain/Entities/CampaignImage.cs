using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class CampaignImage
    {
        [Key]
        public int CampaignImageId { get; set; }

        public int CampaignId { get; set; }

        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }
    }

}
