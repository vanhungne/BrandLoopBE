using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Background
{
    public class AccountCleanupOptions
    {
        public const string SectionName = "AccountCleanup";

        /// <summary>
        /// Có bật tính năng cleanup không. Mặc định: true
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Thời gian kiểm tra (phút). Mặc định: 1 phút
        /// </summary>
        [Range(1, 60)]
        public int CheckIntervalMinutes { get; set; } = 1;

        /// <summary>
        /// Thời gian hết hạn tài khoản chưa verify (phút). Mặc định: 2 phút
        /// </summary>
        [Range(1, 1440)] // Max 1 day
        public int AccountExpiryMinutes { get; set; } = 2;

        /// <summary>
        /// Batch size khi xóa. Mặc định: 50
        /// </summary>
        [Range(1, 1000)]
        public int BatchSize { get; set; } = 50;

        /// <summary>
        /// Delay giữa các batch (milliseconds). Mặc định: 1000ms
        /// </summary>
        [Range(100, 10000)]
        public int BatchDelayMs { get; set; } = 1000;
    }
}
