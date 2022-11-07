namespace IvanGram.Models
{
    public class PostCommentModel
    {
        public DateTimeOffset CreatedAt { get; set; }
        public string Text { get; set; } = null!;
        public string Author { get; set; } = null!;
    }
}
