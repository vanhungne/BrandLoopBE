using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class MessageReadStatus
    {
        [Key]
        public int Id { get; set; }

        public int MessageId { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public DateTime? ReadAt { get; set; }

        // Navigation properties
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }

        [ForeignKey("Email")]
        public virtual User User { get; set; }
    }
}
