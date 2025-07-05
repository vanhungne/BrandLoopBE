using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Config;
using BrandLoop.Infratructure.Models.ChatDTO;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BrandLoop.Application.Service
{
    public sealed class GroqAiService : IChatAiService
    {
        private readonly HttpClient _http;
        private readonly GroqOptions _opt;
        private static readonly JsonSerializerOptions _jOpt =
            new(JsonSerializerDefaults.Web);

        public GroqAiService(HttpClient http, IOptions<GroqOptions> opt)
        {
            _http = http;
            _opt = opt.Value;
        }

        public async Task<ChatResponseDto> AskAsync(ChatRequestDto req,
                                                   CancellationToken ct = default)
        {
            var systemPrompt = !string.IsNullOrEmpty(req.SystemPrompt)
                     ? $"{_opt.DefaultSystemPrompt}\n\n{req.SystemPrompt}"
                     : _opt.DefaultSystemPrompt;

            var body = new
            {
                model = _opt.Model,
                temperature = req.Temperature,
                messages = new[]
                   {
                    new { role = "system", content = systemPrompt },
                    new { role = "user",   content = req.UserPrompt  }
                }
            };

            var httpReq = new HttpRequestMessage(HttpMethod.Post,
                            $"{_opt.BaseUrl}/chat/completions")
            {
                Content = new StringContent(JsonSerializer.Serialize(body, _jOpt),
                                            Encoding.UTF8, "application/json")
            };

            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opt.ApiKey);

            using var res = await _http.SendAsync(httpReq, ct).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();

            using var stream = await res.Content.ReadAsStreamAsync(ct);
            var groq = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var content = groq.RootElement
                              .GetProperty("choices")[0]
                              .GetProperty("message")
                              .GetProperty("content")
                              .GetString()!;

            return new ChatResponseDto { Content = content };
        }
    }
}
