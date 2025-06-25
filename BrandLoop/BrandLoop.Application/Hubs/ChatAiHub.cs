using BrandLoop.Application.Interfaces;
using BrandLoop.Infratructure.Models.ChatDTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Application.Hubs
{
    public class ChatAiHub : Hub
    {
        private readonly IChatAiService _ai;
        private readonly ILogger<ChatAiHub> _logger;

        public ChatAiHub(IChatAiService ai, ILogger<ChatAiHub> logger)
        {
            _ai = ai;
            _logger = logger;
        }

        public async Task SendPrompt(string prompt, string? systemPrompt = null, float temperature = 0.7f)
        {
            try
            {
                if (string.IsNullOrEmpty(prompt))
                {
                    await Clients.Caller.SendAsync("ReceiveError", "Prompt cannot be empty");
                    return;
                }

                // Thông báo đang xử lý
                await Clients.Caller.SendAsync("ReceiveStatus", "Processing...");

                _logger.LogInformation("Processing prompt from connection {ConnectionId}", Context.ConnectionId);

                var request = new ChatRequestDto(
                    prompt,
                    systemPrompt ?? "You are a helpful assistant.",
                    temperature
                );

                var result = await _ai.AskAsync(request, Context.ConnectionAborted);

                // Gửi kết quả về client
                await Clients.Caller.SendAsync("ReceiveAnswer", result.Content);

                _logger.LogInformation("Successfully processed prompt for connection {ConnectionId}", Context.ConnectionId);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Request cancelled for connection {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("ReceiveError", "Request was cancelled");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access for connection {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("ReceiveError", "API key configuration error");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error for connection {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("ReceiveError", "External service error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error for connection {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("ReceiveError", "An unexpected error occurred");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveStatus", "Connected to chat service");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}