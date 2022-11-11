using IvanGram.Models.Attach;

namespace IvanGram.Models.User
{
    public class AddUserAvatarModel
    {
        public MetaDataModel MetaDataModel { get; set; } = null!;
        public Guid UserId { get; set; }
        public string FilePath { get; set; } = null!;
    }
}
