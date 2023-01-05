using Api.Models.User;

namespace Api.Models.Comment
{
    public class CommentModel
    {
        public Guid Id { get; set; }
        public UserModel Author { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; } = false;
    }
}
