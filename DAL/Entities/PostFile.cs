using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostFile : Attach
    {
        public Guid PostId { get; set; }

        public virtual Post Post { get; set; } = null!;
    }
}
