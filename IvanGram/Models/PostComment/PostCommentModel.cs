using IvanGram.Models.User;

namespace IvanGram.Models.PostComment
{
    public class PostCommentModel
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string Text { get; set; } = null!;
        public UserLigthModel Author { get; set; } = null!;
        public int CommentLikeCount { get; set; }
        public bool IsLiked { get; set; } = false;
    }
}
