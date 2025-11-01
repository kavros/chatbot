using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text;
using System.Text.Json;

namespace Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AgentController(
        [FromKeyedServices("webSearchAgent")] AIAgent webSearchAgent
        ) : ControllerBase
    {


        [HttpPost("chat")]
        public async Task<IActionResult> Chat(ChatRequest request)
        {
            string chatHistoryJson = JsonSerializer.Serialize(request.ChatHistory);

            var questionAndChatHistory = request.Message +
                $"The chat history is the following {chatHistoryJson}";

            var response = await webSearchAgent.RunAsync(questionAndChatHistory);
            return Ok(new { response.Text });
        }

        [HttpPost("chat/stream")]
        public async Task ChatStream(ChatRequest request)
        {
            // Add CORS headers for SSE
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            Response.Headers.Append("Access-Control-Allow-Credentials", "true");
            Response.Headers.Append("Access-Control-Allow-Headers", "Content-Type, Authorization, Cookie");

            Response.Headers.ContentType = "text/event-stream";
            Response.Headers.CacheControl = "no-cache";
            Response.Headers.Append("Connection", "keep-alive");

            string chatHistoryJson = JsonSerializer.Serialize(request.ChatHistory);
            var questionAndChatHistory = request.Message +
                $"The chat history is the following {chatHistoryJson}";

            try
            {

                var finalString = new StringBuilder();
                // Generate the streamed agent response(s)
                await foreach (var stream in webSearchAgent.RunStreamingAsync(questionAndChatHistory))
                {
                    finalString.Append(stream);
                    var streamData = JsonSerializer.Serialize(new
                    {
                        text = finalString.ToString(),
                        isComplete = false
                    });

                    await Response.WriteAsync($"data: {streamData}\n\n");
                    await Response.Body.FlushAsync();
                }

                var finalData = JsonSerializer.Serialize(new
                {
                    text = finalString.ToString(),
                    isComplete = true
                });
                await Response.WriteAsync($"data: {finalData}\n\n");
                await Response.Body.FlushAsync();
            }
            catch (Exception)
            {
                var errorData = JsonSerializer.Serialize(new
                {
                    text = "Sorry, I encountered an error while processing your message.",
                    isComplete = true,
                    error = true
                });

                await Response.WriteAsync($"data: {errorData}\n\n");
                await Response.Body.FlushAsync();
            }
        }
    }
}
