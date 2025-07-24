using BrandLoop.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IContactMessageService
    {
        Task<ContactMessageResponseDto> CreateMessageAsync(ContactMessageCreateDto createDto);
        Task<IEnumerable<ContactMessageResponseDto>> GetAllMessagesAsync();
        Task<ContactMessageResponseDto> GetMessageByIdAsync(int id);
        Task<ContactMessageResponseDto> MarkAsReadAsync(int id);
        Task<bool> DeleteMessageAsync(int id);
        Task<int> GetUnreadCountAsync();
    }

}
