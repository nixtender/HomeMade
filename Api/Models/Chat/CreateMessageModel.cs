namespace Api.Models.Chat
{
    public class CreateMessageModel
    {
        public Guid? SenderId { get; set; }
        public Guid ChatId { get; set; }
        public string? Text { get; set; }
    }
}
