using AutoMapper;
using Common;
using Common.Extensions;
using IvanGram.Controllers;
using IvanGram.Models.PostComment;
using IvanGram.Models.User;
using IvanGram.Services;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;

namespace IvanGram
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, DAL.Entities.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => HashHelper.GetHash(src.Password)))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.UtcDateTime))
                ;
            CreateMap<DAL.Entities.User, UserModel>();
            CreateMap<DAL.Entities.UserAvatar, Models.AttachModel>();
            CreateMap<DAL.Entities.PostFile, Models.AttachModel>();
            CreateMap<DAL.Entities.PostComment, PostCommentModel>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author.Id))
                ;
            CreateMap<DAL.Entities.Attach, Models.AttachModel>();

            CreateMap<PostCommentModel, PostCommentWithAvatarLinkModel>();
        }
    }
}
