using DAL.Entities;

namespace IvanGram.Models.PostComment
{
    public class PostCommentModel
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string Text { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public string? AvatarLink { get; set; }
    }
}
