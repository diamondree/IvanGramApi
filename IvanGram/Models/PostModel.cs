namespace IvanGram.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public List<long> AttachesId { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } 
        public string? Description { get; set; }
    }
}
