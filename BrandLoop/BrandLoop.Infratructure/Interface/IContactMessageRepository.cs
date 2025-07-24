using BrandLoop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Interface
{
    public interface IContactMessageRepository
    {
        Task<ContactMessage> CreateAsync(ContactMessage contactMessage);
        Task<IEnumerable<ContactMessage>> GetAllAsync();
        Task<ContactMessage> GetByIdAsync(int id);
        Task<ContactMessage> UpdateAsync(ContactMessage contactMessage);
        Task<bool> DeleteAsync(int id);
        Task<int> GetUnreadCountAsync();
    }
}
