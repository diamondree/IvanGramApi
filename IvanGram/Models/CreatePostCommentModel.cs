namespace IvanGram.Models
{
    public class CreatePostCommentModel
    {
        public Guid PostId { get; set; }
        public string CommentText { get; set; } = null!;
    }
}
