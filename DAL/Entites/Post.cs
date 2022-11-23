using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class Post
    {   //добавить автора
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset Created { get; set; }

        public bool IsExist { get; set; } = true;


        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostPicture>? PostPictures { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual List<LikePost> LikePosts { get; set; } = new List<LikePost>();
    }
}
