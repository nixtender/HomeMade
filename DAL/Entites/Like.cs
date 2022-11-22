using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class Like
    {
        public Guid Id { get; set; }
        
        //public int Count { get; set; }
        //public List<Guid> Users { get; set; } = new List<Guid>();
        public Guid UserId { get; set; }
        //public virtual User UserForLike { get; set; } = null!;

        //public virtual Post PostForLike { get; set; }
        //public virtual Comment CommentForLike { get; set; }
    }
}
