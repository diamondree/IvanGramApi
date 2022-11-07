using DAL;
using DAL.Entities;
using IvanGram.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvanGram.Services
{
    public class PostService : IDisposable
    {
        private readonly DataContext _context;
        private readonly AttachService _attachService;

        public PostService (DataContext context, AttachService attachService)
        {
            _context = context;
            _attachService = attachService;
        }

        public async Task<PostModel> GetPostByPostId(Guid postId)
        {
            var post = await _context.Posts.Include(x=>x.Files).Include(x=>x.Author).Include(x=>x.Comments).FirstOrDefaultAsync(x=>x.Id == postId);
            if (post == null)
                throw new Exception("Post not found");
            var attachModelsList = await _attachService.GetPostAttaches(post);
            var tempList = new List<long>();
            foreach (var attachModel in attachModelsList)
            {
                tempList.Add(attachModel.Id);
            }
            var res = new PostModel
            {
                Id = post.Id,
                CreatedBy = post.Author.Name,
                CreatedAt = post.CreatedAt,
                Description = post.Description,
                AttachesId = tempList,
            };
            
            return res;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
