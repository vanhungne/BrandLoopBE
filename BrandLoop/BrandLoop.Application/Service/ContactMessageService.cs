using BrandLoop.Application.Interfaces;
using BrandLoop.Domain.DTO;
using BrandLoop.Domain.Entities;
using BrandLoop.Infratructure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IContactMessageRepository _repository;

        public ContactMessageService(IContactMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<ContactMessageResponseDto> CreateMessageAsync(ContactMessageCreateDto createDto)
        {
            var contactMessage = new ContactMessage
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                Category = createDto.Category,
                Subject = createDto.Subject,
                Message = createDto.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var createdMessage = await _repository.CreateAsync(contactMessage);
            return MapToResponseDto(createdMessage);
        }

        public async Task<IEnumerable<ContactMessageResponseDto>> GetAllMessagesAsync()
        {
            var messages = await _repository.GetAllAsync();
            return messages.Select(MapToResponseDto);
        }

        public async Task<ContactMessageResponseDto> GetMessageByIdAsync(int id)
        {
            var message = await _repository.GetByIdAsync(id);
            return message != null ? MapToResponseDto(message) : null;
        }

        public async Task<ContactMessageResponseDto> MarkAsReadAsync(int id)
        {
            var message = await _repository.GetByIdAsync(id);
            if (message == null)
                return null;

            message.IsRead = true;
            var updatedMessage = await _repository.UpdateAsync(message);
            return MapToResponseDto(updatedMessage);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _repository.GetUnreadCountAsync();
        }

        private ContactMessageResponseDto MapToResponseDto(ContactMessage message)
        {
            return new ContactMessageResponseDto
            {
                Id = message.Id,
                FullName = message.FullName,
                Email = message.Email,
                Category = message.Category,
                Subject = message.Subject,
                Message = message.Message,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead
            };
        }
    }
}
