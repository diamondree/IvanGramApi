using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class UserAvatar : Attach
    { 
        public long UserAvatarId { get; set; }
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

    }
}
