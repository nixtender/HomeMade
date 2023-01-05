
using Api.Models.User;

namespace Api.Models.Like
{
    public class LikeModel
    {
        public Guid Id { get; set; }
        public Guid ObjectId { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public UserModel User { get; set; } = null!;
    }
}
