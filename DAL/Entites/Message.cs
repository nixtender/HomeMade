using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entites
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatId { get; set; }
        public DateTimeOffset SendingTime { get; set; }
        public string? Text { get; set; }

        public virtual User Sender { get; set; } = null!;
        public virtual Chat Chat { get; set; } = null!;
    }
}
