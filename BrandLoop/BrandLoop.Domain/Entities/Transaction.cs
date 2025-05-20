using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        // Ví nguồn (người gửi)
        public int? FromWalletId { get; set; }

        // Ví đích (người nhận)
        public int? ToWalletId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Loại giao dịch: deposit (nạp tiền), withdraw (rút tiền), 
        // transfer (chuyển tiền), commission (hoa hồng), payment (thanh toán)...
        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; }

        // Trạng thái giao dịch: pending, completed, failed...
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";

        // Mô tả giao dịch
        public string Description { get; set; }

        // ID tham chiếu đến đối tượng liên quan (KolsJoinCampaign, Deal...)
        public int? ReferenceId { get; set; }

        // Bảng tham chiếu: "KolsJoinCampaign", "Deal"...
        [StringLength(100)]
        public string ReferenceTable { get; set; }

        // Số tiền hoa hồng (nếu có)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CommissionAmount { get; set; }

        // Tỷ lệ hoa hồng (nếu có)
        [Column(TypeName = "decimal(5,2)")]
        public decimal? CommissionRate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // Mã giao dịch bên ngoài (nếu có) - từ ngân hàng, ví điện tử...
        [StringLength(100)]
        public string ExternalTransactionCode { get; set; }

        // Navigation properties
        [ForeignKey("FromWalletId")]
        public virtual Wallet FromWallet { get; set; }

        [ForeignKey("ToWalletId")]
        public virtual Wallet ToWallet { get; set; }
    }
}
