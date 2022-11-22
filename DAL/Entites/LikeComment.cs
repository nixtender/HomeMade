using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class LikeComment : Like
    {
        public Guid UserLikeCommentId { get; set; }
        public virtual User UserLikeComment { get; set; }
        //public Guid CommentId { get; set; }
        public virtual Comment Comment { get; set; } = null!;
    }
}
