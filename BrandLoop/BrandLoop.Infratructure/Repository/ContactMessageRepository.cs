using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Repository
{
    public class ContactMessageRepository : IContactMessageRepository
    {
        private readonly BLDBContext _context;

        public ContactMessageRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task<ContactMessage> CreateAsync(ContactMessage contactMessage)
        {
            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();
            return contactMessage;
        }

        public async Task<IEnumerable<ContactMessage>> GetAllAsync()
        {
            return await _context.ContactMessages
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContactMessage> GetByIdAsync(int id)
        {
            return await _context.ContactMessages.FindAsync(id);
        }

        public async Task<ContactMessage> UpdateAsync(ContactMessage contactMessage)
        {
            _context.ContactMessages.Update(contactMessage);
            await _context.SaveChangesAsync();
            return contactMessage;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contactMessage = await _context.ContactMessages.FindAsync(id);
            if (contactMessage == null)
                return false;

            _context.ContactMessages.Remove(contactMessage);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _context.ContactMessages.CountAsync(x => !x.IsRead);
        }
    }
}
