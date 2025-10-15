using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;

namespace Controllers
{
    [ApiController]
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
    }
}
