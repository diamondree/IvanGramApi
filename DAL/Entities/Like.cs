using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Like
    {
        public Guid Id { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual User Author { get; set; } = null!;
    }
}
