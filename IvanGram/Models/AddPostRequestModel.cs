using DAL.Entities;

namespace IvanGram.Models
{
    public class AddPostRequestModel
    {
        public Guid PostId { get; set; }
        public string? Descriprion { get; set; }
        public List<MetaDataModel> Files { get; set; }
        public Guid UserId { get; set; }
        public List<string> FilePath { get; set; }
    }
}
