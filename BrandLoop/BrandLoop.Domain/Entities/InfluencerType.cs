using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class InfluencerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int MinFollower { get; set; }

        [Required]
        public int MaxFollower { get; set; }

        [Required]
        public int PlatformFee { get; set; }

        public ICollection<InfluenceProfile> InfluenceProfiles { get; set; }
    }
}
