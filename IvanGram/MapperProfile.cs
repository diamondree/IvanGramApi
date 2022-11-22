using AutoMapper;
using Common;
using Common.Extensions;
using IvanGram.Controllers;
using IvanGram.Models.Attach;
using IvanGram.Models.Post;
using IvanGram.Models.PostComment;
using IvanGram.Models.Subscribe;
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
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate.UtcDateTime));

            CreateMap<DAL.Entities.User, UserModel>()
                .ForMember(dest => dest.PostsCount, opt => opt.MapFrom(src => src.Posts!.Count()))
                .ForMember(dest => dest.FollowersCount, opt => opt.Ignore())
                .ForMember(dest => dest.SubscribedToCount, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarLink, opt => opt.Ignore());

            CreateMap<DAL.Entities.UserAvatar, AttachModel>();
            CreateMap<DAL.Entities.PostFile, AttachModel>();
            CreateMap<DAL.Entities.Attach, AttachModel>();

            CreateMap<DAL.Entities.PostComment, PostCommentModel>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.Author.Id))
                .ForMember(dest => dest.AvatarLink, opt => opt.Ignore())
                .ForMember(dest => dest.CommentLikeCount, opt => opt.Ignore());

            CreateMap<DAL.Entities.Post, PostModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.AuthorAvatar, opt => opt.Ignore())
                .ForMember(dest => dest.AttachesLinks, opt => opt.Ignore())
                .ForMember(dest => dest.PostLikeCount, opt => opt.Ignore())
                .ForMember(dest => dest.PostCommentCount, opt => opt.Ignore());

            CreateMap<DAL.Entities.Subscription, UnacceptedSubscribeModel>()
                .ForMember(dest => dest.FollowerId, opt => opt.MapFrom(src => src.Follower.Id));
        }
    }
}
