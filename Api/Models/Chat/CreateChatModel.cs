namespace Api.Models.Chat
{
    public class CreateChatModel
    {
        public List<Guid> RecipientIds { get; set; } = new List<Guid>();
        public string? Name { get; set; }
    }
}
