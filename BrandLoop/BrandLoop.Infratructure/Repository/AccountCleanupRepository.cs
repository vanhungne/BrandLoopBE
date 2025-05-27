using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interfaces;
using BrandLoop.Infratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace BrandLoop.Infratructure.Service
{
    public class AccountCleanupRepository : IAccountCleanupRepository
    {
        private readonly BLDBContext _context;
        private IDbContextTransaction _transaction;

        public AccountCleanupRepository(BLDBContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetExpiredUnverifiedAccountsAsync(DateTime cutoffTime, int batchSize)
        {
            return await _context.Users
                .Where(u => u.Status == 0 && u.CreatedAt <= cutoffTime)
                .Include(u => u.BrandProfile)
                .Include(u => u.InfluenceProfile)
                .Include(u => u.RefreshTokens)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task<int> GetExpiredUnverifiedAccountsCountAsync(DateTime cutoffTime)
        {
            return await _context.Users
                .Where(u => u.Status == 0 && u.CreatedAt <= cutoffTime)
                .CountAsync();
        }

        public async Task DeleteUserAndRelatedDataAsync(User user)
        {
            // Xóa brand profile
            if (user.BrandProfile != null)
            {
                _context.BrandProfiles.Remove(user.BrandProfile);
            }

            // Xóa influence profile  
            if (user.InfluenceProfile != null)
            {
                _context.InfluenceProfiles.Remove(user.InfluenceProfile);
            }

            // Xóa refresh tokens
            if (user.RefreshTokens != null && user.RefreshTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(user.RefreshTokens);
            }

            // Xóa notifications
            var notifications = await _context.Notifications
                .Where(n => n.UID == user.UID)
                .ToListAsync();

            if (notifications.Any())
            {
                _context.Notifications.RemoveRange(notifications);
            }

            // Xóa ContentAndStyles
            var contentAndStyles = await _context.ContentAndStyles
                .Where(c => c.UID == user.UID)
                .ToListAsync();
            if (contentAndStyles.Any())
            {
                _context.ContentAndStyles.RemoveRange(contentAndStyles);
            }

            // Xóa Skills
            var skills = await _context.Skills
                .Where(s => s.UID == user.UID)
                .ToListAsync();
            if (skills.Any())
            {
                _context.Skills.RemoveRange(skills);
            }

            // Xóa ConversationParticipants
            var conversationParticipants = await _context.ConversationParticipants
                .Where(cp => cp.UID == user.UID)
                .ToListAsync();
            if (conversationParticipants.Any())
            {
                _context.ConversationParticipants.RemoveRange(conversationParticipants);
            }

            // Xóa Messages
            var messages = await _context.Messages
                .Where(m => m.Sender == user.UID)
                .ToListAsync();
            if (messages.Any())
            {
                _context.Messages.RemoveRange(messages);
            }

            // Xóa MessageReadStatuses
            var messageReadStatuses = await _context.MessageReadStatuses
                .Where(mrs => mrs.UID == user.UID)
                .ToListAsync();
            if (messageReadStatuses.Any())
            {
                _context.MessageReadStatuses.RemoveRange(messageReadStatuses);
            }

            // Xóa SubscriptionRegisters
            var subscriptionRegisters = await _context.SubscriptionRegisters
                .Where(sr => sr.UID == user.UID)
                .ToListAsync();
            if (subscriptionRegisters.Any())
            {
                _context.SubscriptionRegisters.RemoveRange(subscriptionRegisters);
            }

            // Xóa Wallets
            var wallets = await _context.Wallets
                .Where(w => w.UID == user.UID)
                .ToListAsync();
            if (wallets.Any())
            {
                _context.Wallets.RemoveRange(wallets);
            }

            // Cuối cùng xóa user
            _context.Users.Remove(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}
