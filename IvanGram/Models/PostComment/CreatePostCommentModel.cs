namespace IvanGram.Models.PostComment
{
    public class CreatePostCommentModel
    {
        public Guid PostId { get; set; }
        public string CommentText { get; set; } = null!;
    }
}
