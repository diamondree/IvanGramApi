using AutoMapper;
using DAL.Entities;
using IvanGram.Models.User;
using IvanGram.Services;

namespace IvanGram.Mapper.MapperAction
{
    public class UserModelAvatarMapperAction : IMappingAction<User, UserModel>
    {
        private LinkGeneratorService _links;
        public UserModelAvatarMapperAction(LinkGeneratorService links)
        {
            _links = links;
        }
        public void Process(User source, UserModel destination, ResolutionContext context) 
            => _links.FixUserAvatar(source, destination);
    }
}
