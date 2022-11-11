﻿namespace IvanGram.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string Author { get; set; } = null!;
        public string? AuthorAvatar { get; set; }
        public List<string> AttachesLinks { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public string? Description { get; set; }
    }
}
