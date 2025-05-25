using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BrandLoop.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;
        [StringLength(50)]
        public string WalletType { get; set; } = "main";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("Email")]
        public virtual User User { get; set; }

        public virtual ICollection<Transaction> FromTransactions { get; set; }
        public virtual ICollection<Transaction> ToTransactions { get; set; }
    }
}
