using Common.Consts;
using Common.Extensions;
using DAL.Entities;
using IvanGram.Models.Post;
using IvanGram.Models.PostComment;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly UserService _userService;

        public PostController(PostService postService, UserService userService)
        {
            _postService = postService;
            _postService.SetLinkGenerator(_linkContentGenerator, _linkAvatarGenerator);
            _userService = userService;
        }

        private string? _linkAvatarGenerator(Guid userId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId,
            });
        }

        private string? _linkContentGenerator(Guid postContentId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task CreatePost(AddPostModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _userService.CreateUserPost(model, userId);
            }
            else
                throw new Exception("You are not authorized");
        }

        [HttpGet]
        public async Task<PostModel> GetPostByPostId(Guid PostId) 
            => await _postService.GetPostByPostId(PostId);

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10)
            => await _postService.GetPosts(skip, take);

        [HttpPost]
        [Authorize]
        public async Task AddCommentToPost(CreatePostCommentModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _postService.CreatePostComment(userId, model);
            }
            else
                throw new Exception("You are not authorized");
        }

        [HttpGet]
        public async Task<List<PostCommentModel>> GetPostComments(Guid PostId) 
            => await _postService.GetPostComments(PostId);
    }
}
