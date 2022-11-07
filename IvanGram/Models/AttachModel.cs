namespace IvanGram.Models
{
    public class AttachModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
}
