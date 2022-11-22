using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class LikePost : Like
    {
        public Guid UserLikePostId { get; set; }
        public virtual User UserLikePost { get; set; }
        //public Guid PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
