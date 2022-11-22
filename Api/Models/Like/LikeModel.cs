namespace Api.Models.Like
{
    public class LikeModel
    {
        int Count { get; set; }
        List<Guid> Users { get; set; } = new List<Guid>();

        public LikeModel(int count, List<Guid> users)
        {
            Count = count;
            Users = users;
        }
    }
}
