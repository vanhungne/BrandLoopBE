using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class RefreshTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RId { get; set; }

        public string? Token { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public DateTime Expires { get; set; }
        [ForeignKey("Email")]
        public User? User { get; set; }

    }
}
