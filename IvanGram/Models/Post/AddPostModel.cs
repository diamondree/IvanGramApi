using IvanGram.Models.Attach;

namespace IvanGram.Models.Post
{
    public class AddPostModel
    {
        public List<MetaDataModel> Files { get; set; }
        public string Description { get; set; }
    }
}
