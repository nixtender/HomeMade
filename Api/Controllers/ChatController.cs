using Api.Models.Chat;
using Api.Models.Push;
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
        private readonly UserService _userService;
        private readonly GooglePushService _googlePushService;

        public ChatController(ChatService chatService, LinkGeneratorService links, UserService userService, GooglePushService googlePushService)
        {
            _chatService = chatService;

            links.LinkAvatarGenerator = x =>
            Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
            _userService = userService;
            _googlePushService = googlePushService;
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

        [HttpGet]
        public async Task<ChatModel> GetCertainChat(Guid chatId) => await _chatService.GetCertainChat(chatId);

        [HttpPost]
        public async Task SendMessage(CreateMessageModel model)
        {
            var res = new List<string>();
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            var user = await _userService.GetUserById(userId);
            var chat = await _chatService.GetCertainChat(model.ChatId);
            if (chat != null)
            {
                var otherIds = new List<Guid>();
                foreach(var chUser in chat.Users)
                {
                    otherIds.Add(chUser.Id);
                }
                otherIds.Remove(userId);

                model.SenderId = userId;
                await _chatService.SendMessage(model);

                foreach(var otherId in otherIds)
                {
                    var token = await _userService.GetPushToken(otherId);
                    if (token != default)
                    {
                        var pushModel = new PushModel { Alert = new PushModel.AlertModel { Title = user.Name, Subtitle = "subtitle", Body = model.Text }, Badge = 0, Sound = "string", CustomData = new Dictionary<string, object>() { { "commandName", "getMessage" }, { "commandArg", chat.Id } } };
                        //var sendPushModel = new SendPushModel { UserId = userId, Push = pushModel };

                        res = _googlePushService.SendNotification(token, pushModel);
                    }
                }
                
                
            }
            //var userIdT = model.UserId ?? userId;
            
            /*return res;


            if (userId != default)
            {
                model.SenderId = userId;
                await _chatService.SendMessage(model);
            }
            else throw new Exception("user is not found");*/
        }

        [HttpGet]
        public async Task<List<MessageModel>> GetMessages(Guid chatId) => await _chatService.GetMessages(chatId);
    }
}
