namespace Api.Models.Chat
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatId { get; set; }
        public DateTimeOffset SendingTime { get; set; }
        public string Text { get; set; } = null!;
    }
}
