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
        public int Count { get; set; }
        public List<Guid> Users { get; set; } = new List<Guid>();

        public virtual Post PostForLike { get; set; }
    }
}
