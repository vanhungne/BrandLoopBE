using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.DTO
{
    public class ContactMessageCreateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class ContactMessageResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
