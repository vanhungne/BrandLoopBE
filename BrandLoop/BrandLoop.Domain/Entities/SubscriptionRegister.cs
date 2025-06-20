﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Shared.Helper;
using BrandLoop.Domain.Enums;

namespace BrandLoop.Domain.Entities
{
    public class SubscriptionRegister
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; }

        public int SubscriptionId { get; set; }

        [StringLength(50)]
        public RegisterSubStatus Status { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTimeHelper.GetVietnamNow();

        public DateTime? ExpirationDate { get; set; }

        // Navigation properties
        [ForeignKey("UID")]
        public virtual User User { get; set; }

        [ForeignKey("SubscriptionId")]
        public virtual Subscription Subscription { get; set; }

        public virtual ICollection<Payment> Payments { get; set; }
    }

}
