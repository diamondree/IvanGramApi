using DAL.Entities;

namespace IvanGram.Models.Subscribe
{
    public class UnacceptedSubscribeModel
    {
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
    }
}
