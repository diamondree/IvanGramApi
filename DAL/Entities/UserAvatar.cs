using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class UserAvatar : Attach
    { 
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;

    }
}
