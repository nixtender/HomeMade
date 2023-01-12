using Api.Models.Comment;
using Api.Models.Post;
using Api.Models.User;

namespace Api.Models.Like
{
    public class LikeCommentModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public UserModel User { get; set; } = null!;
        public CommentModel Comment { get; set; } = null!;
    }
}
