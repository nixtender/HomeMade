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

        //public Guid UserLikeId { get; set; }
        public virtual User UserLike { get; set; }
    }
}
