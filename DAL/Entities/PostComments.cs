using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostComments
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public virtual Post Post { get; set; }
        public virtual User Author { get; set; }
    }
}
