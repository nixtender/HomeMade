using Api.Models.Chat;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task CreateChat(CreateChatModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _chatService.CreateChat(model, userId);
        }

        [HttpPost]
        public async Task SendMessage(CreateMessageModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                model.SenderId = userId;
                await _chatService.SendMessage(model);
            }
            else throw new Exception("user is not found");
        }
    }
}
