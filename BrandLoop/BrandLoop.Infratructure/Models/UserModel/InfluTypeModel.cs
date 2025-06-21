using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.UserModel
{
    public class InfluTypeModel
    {
        public int? Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int MinFollower { get; set; }

        [Required]
        public int MaxFollower { get; set; }

        [Required]
        public int PlatformFee { get; set; }
    }
}
