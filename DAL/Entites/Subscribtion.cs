using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class Subscribtion
    {
        public Guid Id { get; set; }
        public Guid PublisherId { get; set; }
        public Guid FollowerId { get; set; }
        public DateTimeOffset SubscriptionDate { get; set; }

        public virtual User Publisher { get; set; } = null!;
        public virtual User Follower { get; set; } = null!;
    }
}
