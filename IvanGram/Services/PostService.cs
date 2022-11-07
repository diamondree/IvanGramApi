using AutoMapper;
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
        private readonly IMapper _mapper;

        public PostService (DataContext context, AttachService attachService, IMapper mapper)
        {
            _context = context;
            _attachService = attachService;
            _mapper = mapper;
        }

        public async Task<PostModel> GetPostByPostId(Guid postId)
        {
            var post = await _context.Posts.Include(x=>x.Files).Include(x=>x.Author).FirstOrDefaultAsync(x=>x.Id == postId);
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

        public async Task<List<PostCommentModel>> GetPostComments (Guid postId)
        {
            var post = await _context.Posts.Include(x => x.Comments).ThenInclude(x=>x.Author).FirstOrDefaultAsync(x=>x.Id == postId);
            if (post == null)
                throw new Exception("Post does not exists");
            if (post.Comments == null)
                throw new Exception("Post comments not found");
            if (post.Comments.Count <= 0)
                throw new Exception("There are not any comments in this post");
            var comments = new List<PostCommentModel>();
            foreach (var comment in post.Comments)
            {
                comments.Add(_mapper.Map<PostCommentModel>(comment));
            }
            return comments;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
