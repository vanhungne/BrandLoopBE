using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Infratructure.Models.ChatDTO
{
    public record ChatRequestDto(string UserPrompt,
                                 string SystemPrompt = "You are a helpful assistant.",
                                 float Temperature = 0.7f);

    public record ChatResponseDto
    {
        public string Content { get; init; } = default!;
    }
}
