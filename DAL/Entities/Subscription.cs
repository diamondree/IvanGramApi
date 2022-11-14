using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Subscription
    {
        public bool IsActive { get; set; }
        public DateTimeOffset SubscribedAt { get; set; }
        public virtual User Follower { get; set; } = null!;
        public virtual User SubscribedTo { get; set; } = null!;
    }
}