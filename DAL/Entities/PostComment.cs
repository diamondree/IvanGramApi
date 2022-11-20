namespace DAL.Entities
{
    public class PostComment
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }

        public virtual Post Post { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostCommentLike>? CommentLikes { get; set; }
    }
}
