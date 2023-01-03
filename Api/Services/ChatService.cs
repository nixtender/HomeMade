using Api.Models.Chat;
using AutoMapper;
using DAL;
using DAL.Entites;

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
            if (recipients.Count == 1 && model.Name == null)
                dbChat.Name = recipients[0].Name;
            else dbChat.Name = model.Name;
            recipients.Add(await _userService.GetUserById(userId));
            dbChat.Users = recipients;
            await _context.Chats.AddAsync(dbChat);
            await _context.SaveChangesAsync();
        }

        public async Task SendMessage(CreateMessageModel model)
        {
            var dbMessage = _mapper.Map<Message>(model);
            await _context.Messages.AddAsync(dbMessage);
            await _context.SaveChangesAsync();
        }
    }
}
