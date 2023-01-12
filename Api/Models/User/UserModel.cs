namespace Api.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string? AvatarLink { get; set; }
        public int PostCount { get; set; }
        public int PublisherCount { get; set; }
        public int FollowerCount { get; set; }

        /*public UserModel(Guid id, string name, string email, DateTimeOffset birthDate, int postCount)
        {
            Id = id;
            Name = name;
            Email = email;
            BirthDate = birthDate;
            PostCount = postCount;
        }*/
    }
}
