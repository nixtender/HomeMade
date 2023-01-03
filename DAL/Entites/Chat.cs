using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTimeOffset CreateChatTime { get; set; }
        public DateTimeOffset EndMessageTime { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
