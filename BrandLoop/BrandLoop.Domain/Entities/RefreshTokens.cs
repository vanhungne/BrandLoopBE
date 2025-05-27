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

        [StringLength(32)]
        public string UID { get; set; }

        public DateTime Expires { get; set; }
        [ForeignKey("UID")]
        public User? User { get; set; }

    }
}
