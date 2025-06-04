using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Enums
{
    public enum RegisterSubStatus
    {
        Pending,       // Đang chờ xử lý
        Active,        // Đã kích hoạt
        Expired,       // Đã hết hạn
        Cancelled,     // Đã hủy
        Failed         // Thất bại trong quá trình đăng ký
    }
}
