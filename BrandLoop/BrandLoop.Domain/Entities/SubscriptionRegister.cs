using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class SubscriptionRegister
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public int SubscriptionId { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public DateTime? ExpirationDate { get; set; }

        // Navigation properties
        [ForeignKey("UserName")]
        public virtual User User { get; set; }

        [ForeignKey("SubscriptionId")]
        public virtual Subscription Subscription { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }

}
