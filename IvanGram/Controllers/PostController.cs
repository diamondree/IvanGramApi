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
    [ApiExplorerSettings(GroupName = "Api")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly UserService _userService;

        public PostController(PostService postService, UserService userService, LinkGeneratorService links)
        {
            _postService = postService;
            _userService = userService;
            links.LinkAvatarGenerator = x=> Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
            links.LinkPostContentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId = x.Id,
            });
        }

        [HttpPost]
        public async Task CreatePost(AddPostModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _userService.CreateUserPost(model, userId);
            }
            else
                throw new System.Exception("You are not authorized");
        }

        [HttpGet]
        public async Task<PostModel> GetPostByPostId(Guid PostId)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetPostByPostId(PostId, currentUserId);
        }

        [HttpGet]
        public async Task<List<PostModel>> GetAllPosts(int skip = 0, int take = 10)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetAllPosts(skip, take, currentUserId);
        }

        [HttpGet]
        public async Task<List<PostModel>> GetUserFolowedUsersPosts()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetUserFolowedUsersPosts(userId);
        }

        [HttpGet]
        public async Task<List<PostModel>> GetUserPosts (Guid userId)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetUserPosts(userId, currentUserId);
        }

        [HttpPost]
        public async Task AddCommentToPost(CreatePostCommentModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                await _postService.CreatePostComment(userId, model);
            }
            else
                throw new System.Exception("You are not authorized");
        }

        [HttpGet]
        public async Task<List<PostCommentModel>> GetPostComments(Guid PostId)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _postService.GetPostComments(PostId, currentUserId);
        }
        [HttpPost]
        public async Task LikePost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _postService.LikePost(postId, userId);
        }

        [HttpPost]
        public async Task DislikePost(Guid postId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _postService.DislikePost(postId, userId);
        }

        [HttpPost]
        public async Task LikePostComment(Guid postCommentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _postService.LikePostComment(postCommentId, userId);
        }

        [HttpPost]
        public async Task DislikePostComment(Guid postCommentId)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _postService.DislikePostComment(postCommentId, userId);
        }
    }
}
