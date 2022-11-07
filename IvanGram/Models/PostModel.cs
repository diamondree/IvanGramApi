namespace IvanGram.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public List<AttachModel> Files { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; } 
        public string? Description { get; set; }

        public PostModel(Guid id, string createdBy, List<AttachModel> files, DateTimeOffset createdAt, string? description)
        {
            Id = id;
            CreatedBy = createdBy;
            Files = files;
            CreatedAt = createdAt;
            Description = description;
        }
    }
}
