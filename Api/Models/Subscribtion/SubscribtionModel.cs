namespace Api.Models.Subscribtion
{
    public class SubscribtionModel
    {
        public Guid Id { get; set; }
        public Guid PublisherId { get; set; }
        public Guid FollowerId { get; set; }
        public DateTimeOffset SubscriptionDate { get; set; }
    }
}
