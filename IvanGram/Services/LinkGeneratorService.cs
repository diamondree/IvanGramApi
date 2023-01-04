using DAL.Entities;
using IvanGram.Models.Attach;
using IvanGram.Models.Post;
using IvanGram.Models.User;

namespace IvanGram.Services
{
    public class LinkGeneratorService
    {
        public Func<User, string?>? LinkAvatarGenerator;
        public Func<PostFile, string?>? LinkPostContentGenerator;

        public void FixUserLightAvatar(User source, UserLigthModel destination)
        {
            destination.AvatarLink = source.Avatar == null ? null : LinkAvatarGenerator?.Invoke(source);
        }

        public void FixUserAvatar(User source, UserModel destination)
        {
            destination.AvatarLink = source.Avatar == null ? null : LinkAvatarGenerator?.Invoke(source);
        }

        public void FixPostContent(PostFile source, AttachExternalModel destination)
        {
            destination.ContentLink = LinkPostContentGenerator?.Invoke(source);
        }
    }
}
