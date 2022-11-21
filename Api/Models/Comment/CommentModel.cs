using Api.Models.User;

namespace Api.Models.Comment
{
    public class CommentModel
    {
        public UserModel Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
    }
}
