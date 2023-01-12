using Api.Models.Chat;
using Api.Models.User;
using AutoMapper;
using DAL;
using DAL.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class ChatService
    {
        private readonly DAL.DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserService _userService;

        public ChatService(DataContext context, IMapper mapper, UserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task CreateChat(CreateChatModel model, Guid userId)
        {
            List<User> recipients = new List<User>();
            foreach (var recipientId in model.RecipientIds)
            {
                recipients.Add(await _userService.GetUserById(recipientId));
            }
            var dbChat = new Chat { CreateChatTime = DateTime.UtcNow, EndMessageTime = DateTime.UtcNow };
            dbChat.Name = model.Name;
            recipients.Add(await _userService.GetUserById(userId));
            dbChat.Users = recipients;
            await _context.Chats.AddAsync(dbChat);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatModel>> GetChats(Guid userId)
        {
            var chats = await _context.Chats
                .Include(x => x.Users).ThenInclude(x => x.Avatar)
                .Include(x => x.Messages)
                .Where(x => x.Users.Any(y => y.Id == userId))
                .OrderByDescending(x => x.EndMessageTime)
                .AsNoTracking()
                .ToListAsync();
            List<ChatModel> chatModels = new List<ChatModel>();
            foreach (var chat in chats)
            {
                var chatModel = _mapper.Map<ChatModel>(chat);
                chatModels.Add(chatModel);
            }
            return chatModels;
        }

        public async Task<ChatModel> GetChat(Guid userId, Guid otherUserId)
        {
            var chats = await _context.Chats
                .Include(x => x.Users).ThenInclude(x => x.Avatar)
                .Include(x => x.Messages)
                .Where(x => x.Users.Any(y => y.Id == userId) && x.Name == null)
                .AsNoTracking()
                .ToListAsync();
            if (chats == null)
                return new ChatModel { Id = Guid.NewGuid(), Name = "newnew", CreateChatTime = DateTime.UtcNow, EndMessageTime = DateTime.UtcNow };
            var curChat = chats.Where(x => x.Users.Any(y => y.Id == otherUserId)).FirstOrDefault();
            if (curChat == null || curChat == default)
                return new ChatModel { Id = Guid.NewGuid(), Name = "newnew", CreateChatTime = DateTime.UtcNow, EndMessageTime = DateTime.UtcNow };
            return _mapper.Map<ChatModel>(curChat);
        }

        public async Task SendMessage(CreateMessageModel model)
        {
            var dbMessage = _mapper.Map<Message>(model);
            var chat = await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == model.ChatId);
            if (chat != null)
            {
                chat.EndMessageTime = dbMessage.SendingTime;
                _context.Chats.Update(chat);
            }
            await _context.Messages.AddAsync(dbMessage);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MessageModel>> GetMessages(Guid chatId)
        {
            var chat = await _context.Chats.AsNoTracking().Include(x => x.Messages).Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == chatId);
            if (chat != null)
            {
                var messageModels = chat.Messages
                    .OrderByDescending(x => x.SendingTime)
                    .Select(x => _mapper.Map<MessageModel>(x))
                    .ToList();
                return messageModels;
            }
            throw new Exception("chat is not found");
        }
    }
}
