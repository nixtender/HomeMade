
using Api.Models.Post;
using Api.Models.User;

namespace Api.Models.Like
{
    public class LikePostModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public UserModel User { get; set; } = null!;
        public PostModel Post { get; set; } = null!;
    }
}
