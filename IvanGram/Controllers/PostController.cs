using IvanGram.Models;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        }

        [HttpGet]
        public async Task<PostModel> GetPostByPostId(Guid PostId)
        {
            return await _postService.GetPostByPostId(PostId);
        }

        [HttpPost]
        [Authorize]
        public async Task AddCommentToPost(CreatePostCommentModel model)
        {
            var userIdStr = User.Claims.FirstOrDefault(x=>x.Type == "Id")?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
            {
                await _postService.CreatePostComment(userId, model);
            }
            else
                throw new Exception("You are not authorized");
        }
    }
}
