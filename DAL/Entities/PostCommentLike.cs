using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostCommentLike : Like
    {
        public Guid PostCommentId { get; set; }

        public virtual PostComment PostComment { get; set; } = null!;
    }
}
