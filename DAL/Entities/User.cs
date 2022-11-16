using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Empty";
        public string Email { get; set; } = "Empty";
        public string PasswordHash { get; set; } = "Empty";
        public DateTimeOffset BirthDate { get; set; }
        public bool IsPrivate { get; set; } = false;

        public virtual UserAvatar? Avatar { get; set; }
        public virtual ICollection<UserSession>? Sessions { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
    }
}
