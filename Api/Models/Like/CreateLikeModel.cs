namespace Api.Models.Like
{
    public class CreateLikeModel
    {
        public Guid ObjectId { get; set; }
        public Guid? UserId { get; set; }
    }
}
