using BrandLoop.Domain.Enums;
using BrandLoop.Shared.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.SubcriptionModel
{
    public class SubscriptionRegisterDTO
    {
        public int Id { get; set; }

        public string UID { get; set; }

        public int SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public string Description { get; set; }

        [StringLength(50)]
        public RegisterSubStatus Status { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? ExpirationDate { get; set; }
    }
}
