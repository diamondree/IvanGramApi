using AutoMapper;
using Common;

namespace IvanGram
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.CreateUserModel, DAL.Entities.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashHelper.GetHash(src.Password)))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.UtcDateTime))
                ;
            CreateMap<DAL.Entities.User, Models.UserModel>();
            CreateMap<DAL.Entities.UserAvatar, Models.AttachModel>();
            CreateMap<DAL.Entities.PostFiles, Models.AttachModel>();
            CreateMap<DAL.Entities.PostComments, Models.PostCommentModel>()
                .ForMember(dest=>dest.Author, opt=>opt.MapFrom(src=>src.Author.Name))
                ;
        }
    }
}
