using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostComment
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
    }
}
