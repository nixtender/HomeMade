using DAL.Entites;
using Microsoft.AspNetCore.Mvc;
using Api.Models.User;

namespace Api.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public ICollection<Attach.AttachExternalModel>? Pictures { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedPost { get; set; }
        public UserModel Author { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }

        /*public PostModel(ICollection<string>? pictures, string? description, DateTimeOffset createdPost, UserModel author, int commentCount)
        {
            Pictures = pictures;
            Description = description;
            CreatedPost = createdPost;
            Author = author;
            CommentCount = commentCount;
        }*/
    }
}
