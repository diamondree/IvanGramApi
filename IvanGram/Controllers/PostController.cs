using IvanGram.Models;
using IvanGram.Services;
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
    }
}
