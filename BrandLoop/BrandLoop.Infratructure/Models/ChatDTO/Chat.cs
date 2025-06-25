using BrandLoop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.ChatDTO
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public string Title { get; set; }
        public ConversationType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public MessageDto LastMessage { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public int UnreadCount { get; set; }
    }

    public class CreateConversationRequest
    {
        public string Title { get; set; }
        public ConversationType Type { get; set; } = ConversationType.Direct;
        public List<string> ParticipantUids { get; set; }
        public string CreatedBy { get; set; }
    }
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatar { get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; }
        public int? ReplyToMessageId { get; set; }
        public MessageDto ReplyToMessage { get; set; }
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public MessageStatus Status { get; set; }
        public List<ReadStatusDto> ReadStatuses { get; set; }
    }

    public class SendMessageRequest
    {
        public int ConversationId { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public int? ReplyToMessageId { get; set; }
        public string AttachmentUrl { get; set; }
        public string AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
    }

    public class ReadStatusDto
    {
        public string UID { get; set; }
        public string UserName { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class ParticipantDto
    {
        public string UID { get; set; }
        public string FullName { get; set; }
        public string ProfileImage { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class UserOnlineStatusDto
    {
        public string UID { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
