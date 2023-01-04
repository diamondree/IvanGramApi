using AutoMapper;
using DAL.Entities;
using IvanGram.Models.User;
using IvanGram.Services;

namespace IvanGram.Mapper.MapperAction
{
    public class UserLightAvatarMapperAction : IMappingAction<User, UserLigthModel>
    {
        private LinkGeneratorService _links;
        public UserLightAvatarMapperAction(LinkGeneratorService links)
        {
            _links = links;
        }

        public void Process(User source, UserLigthModel destination, ResolutionContext context)
            => _links.FixUserLightAvatar(source, destination);
    }
}
