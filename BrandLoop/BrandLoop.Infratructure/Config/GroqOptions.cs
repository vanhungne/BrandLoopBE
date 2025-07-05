using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Config
{
    public sealed class GroqOptions
    {
        public const string SectionName = "Groq";
        public string ApiKey { get; init; } = default!;
        public string BaseUrl { get; init; } = "https://api.groq.com/openai/v1";
        public string Model { get; init; } = "llama3-70b-8192";

        // System prompt mặc định cho BrandLoop
        public string DefaultSystemPrompt { get; init; } = @"
Bạn là trợ lý AI của BrandLoop - nền tảng kết nối KOL/Influencer với các thương hiệu.

THÔNG TIN CƠ BẢN VỀ BRANDLOOP:
- Tên: BrandLoop
- Chức năng: Kết nối KOL/Influencer với các thương hiệu
- Mục tiêu: Tạo cầu nối hiệu quả giữa người có ảnh hưởng và doanh nghiệp
- Dịch vụ: Quản lý chiến dịch marketing, tìm kiếm KOL phù hợp, theo dõi hiệu quả

NHIỆM VỤ CỦA BẠN:
1. Luôn giới thiệu về BrandLoop khi được hỏi về hệ thống
2. Hỗ trợ người dùng tìm hiểu về dịch vụ
3. Giải đáp các thắc mắc về marketing influencer
4. Tư vấn về việc kết nối KOL và thương hiệu

Hãy trả lời một cách thân thiện, chuyên nghiệp và luôn liên kết với mục tiêu của BrandLoop.";
    }
}