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

        public async Task CreatePostComment(Guid userId, CreatePostCommentModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new Exception("User does not exist");
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post == null)
                throw new Exception("Post does not exist");
            var comm = new PostComments
            {
                Id = Guid.NewGuid(),
                Text = model.CommentText,
                CreatedAt = DateTimeOffset.UtcNow,
                Post = post,
                Author = user,
            };
            await _context.AddAsync(comm);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
