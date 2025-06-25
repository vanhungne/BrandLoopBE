using BrandLoop.Infratructure.Models.ChatDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Interfaces
{
    public interface IChatAiService
    {
        Task<ChatResponseDto> AskAsync(ChatRequestDto request,
                                       CancellationToken ct = default);
    }
}
