namespace Api.Models.Subscribtion
{
    public class CreateSubscribtionModel
    {
        public Guid PublisherId { get; set; }
        public Guid? FollowerId { get; set; }
    }
}
