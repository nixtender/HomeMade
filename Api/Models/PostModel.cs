using DAL.Entites;
using Microsoft.AspNetCore.Mvc;

namespace Api.Models
{
    public class PostModel
    {
        public ICollection<string>? Pictures { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedPost { get; set; }
        public string Author { get; set; }

        public PostModel(ICollection<string>? pictures, string? description, DateTimeOffset createdPost, string author)
        {
            Pictures = pictures;
            Description = description;
            CreatedPost = createdPost;
            Author = author;
        }
    }
}
