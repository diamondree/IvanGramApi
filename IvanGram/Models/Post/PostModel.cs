using IvanGram.Models.Attach;
using IvanGram.Models.User;

namespace IvanGram.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public UserLigthModel Author { get; set; } = null!;
        public List<AttachExternalModel> Attaches { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public string? Description { get; set; }
        public int PostLikeCount { get; set; }
        public int PostCommentCount { get; set; }
        public bool IsLiked { get; set; } = false;
    }
}
