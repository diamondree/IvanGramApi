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

        public PostController(PostService postService)
        {
            _postService = postService;
            _postService.SetLinkGenerator(_linkContentGenerator, _linkAvatarGenerator);
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
        public async Task<List<PostCommentWithAvatarLinkModel>> GetPostComments(Guid PostId) 
            => await _postService.GetPostComments(PostId);
    }
}
