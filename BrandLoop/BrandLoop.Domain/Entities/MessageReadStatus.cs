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

        [StringLength(32)]
        public string UID { get; set; }

        public DateTime? ReadAt { get; set; }

        // Navigation properties
        [ForeignKey("MessageId")]
        public virtual Message Message { get; set; }

        [ForeignKey("UID")]
        public virtual User User { get; set; }
    }
}
