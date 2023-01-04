using System.ComponentModel.DataAnnotations;

namespace IvanGram.Models.User
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? AvatarLink { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public int FollowersCount { get; set; }
        public int SubscribedToCount { get; set; }
        public int PostsCount { get; set; }
    }

    public class UserLigthModel
    {
        public Guid Id { get; set; }
        public string? AvatarLink { get; set; }
        public string Name { get; set; } = null!;
    }
}
