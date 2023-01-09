using Api.Models.User;

namespace Api.Models.Chat
{
    public class ChatModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset CreateChatTime { get; set; }
        public DateTimeOffset EndMessageTime { get; set; }
        public string? EndMessage { get; set; }
        //public Guid? SecondUserId { get; set; }
        public ICollection<UserModel>? Users { get; set; }
        //public ICollection<UserModel>? Users { get; set; }
    }
}
