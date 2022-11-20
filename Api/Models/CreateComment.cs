namespace Api.Models
{
    public class CreateComment
    {
        public Guid PostId { get; set; }
        public string Description { get; set; } = null!;

    }
}
