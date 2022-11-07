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
            var post = await _context.Posts.Include(x=>x.Files).Include(x=>x.Comments).Include(x=>x.Author).FirstOrDefaultAsync(x=>x.Id == postId);
            if (post == null)
                throw new Exception("Post not found");
            var attachList = new List<AttachModel>();
            attachList = await _attachService.GetFilesFromPost(post);
            var postModel = new PostModel(postId, post.Author.Name, attachList, post.CreatedAt, post.Description);
            return postModel;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
