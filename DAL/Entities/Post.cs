using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? Description { get; set; }

        public virtual User Author { get; set; }
        public virtual ICollection<PostFiles> Files { get; set; }
        public virtual ICollection<PostComments>? Comments { get; set; }
    }
}
