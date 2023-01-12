using Api.Models.Chat;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using DAL.Entites;
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

        public ChatController(ChatService chatService, LinkGeneratorService links)
        {
            _chatService = chatService;

            links.LinkAvatarGenerator = x =>
            Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [HttpPost]
        public async Task CreateChat(CreateChatModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _chatService.CreateChat(model, userId);
        }

        [HttpGet]
        public async Task<List<ChatModel>> GetChats()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            var chatModels = await _chatService.GetChats(userId);
            return chatModels;
        }

        [HttpGet]
        public async Task<ChatModel> GetChat(Guid otherUserId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _chatService.GetChat(userId, otherUserId);
        }

        [HttpPost]
        public async Task SendMessage(CreateMessageModel model)
        {
            var res = new List<string>();
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var userIdT = model.UserId ?? userId;
                var token = await _userService.GetPushToken(userIdT);
                if (token != default)
                {
                    res =  _googlePushService.SendNotification(token, model.Push);
                }
            }
            return res;


            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                model.SenderId = userId;
                await _chatService.SendMessage(model);
            }
            else throw new Exception("user is not found");
        }

        [HttpGet]
        public async Task<List<MessageModel>> GetMessages(Guid chatId) => await _chatService.GetMessages(chatId);
    }
}
