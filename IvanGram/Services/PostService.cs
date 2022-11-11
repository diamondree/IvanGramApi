using AutoMapper;
using DAL;
using DAL.Entities;
using IvanGram.Models;
using IvanGram.Models.Post;
using IvanGram.Models.PostComment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvanGram.Services
{
    public class PostService : IDisposable
    {
        private readonly DataContext _context;
        private readonly AttachService _attachService;
        private readonly IMapper _mapper;
        private Func<Guid, string?>? _linkContentGenerator;
        private Func<Guid, string?>? _linkAvatarGenerator;

        public void SetLinkGenerator(Func<Guid, string?> linkContentGenerator, Func<Guid, string?> linkAvatarGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkContentGenerator = linkContentGenerator;
        }

        public PostService (DataContext context, AttachService attachService, IMapper mapper)
        {
            _context = context;
            _attachService = attachService;
            _mapper = mapper;
        }

        public async Task<PostModel> GetPostByPostId(Guid postId)
        {
            var post = await _context.Posts
                .Include(x=>x.Files)
                .Include(x=>x.Author).ThenInclude(x=>x.Avatar)
                .FirstOrDefaultAsync(x=>x.Id == postId);
            if (post == null)
                throw new Exception("Post not found");
            var attachModelsList = await _attachService.GetPostAttaches(post);
            var tempList = new List<string>();
            foreach (var attachModel in attachModelsList)
            {
                tempList.Add(_linkContentGenerator(attachModel.Id));
            }
            var temp = string.Empty;
            if (post.Author.Avatar != null)
                temp = _linkAvatarGenerator(post.Author.Id);
            var res = new PostModel
            {
                Id = post.Id,
                Author = post.Author.Name,
                AuthorAvatar = temp,
                CreatedAt = post.CreatedAt,
                Description = post.Description,
                AttachesLinks = tempList,
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
            var comm = new PostComment
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

        public async Task<List<PostCommentWithAvatarLinkModel>> GetPostComments (Guid postId)
        {
            var post = await _context.Posts
                .Include(x => x.Comments).ThenInclude(x => x.Author).ThenInclude(x=>x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == postId);

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

            var commentsWithAvatarLink = new List<PostCommentWithAvatarLinkModel>();

            foreach (var comment in comments)
            {
                commentsWithAvatarLink.Add(_mapper.Map<PostCommentModel, PostCommentWithAvatarLinkModel > (comment, opt =>
                {
                    opt.AfterMap((src, dest) => dest.AvatarLink = FixAvatar(src.AuthorId));
                }));
            }
            return commentsWithAvatarLink;
        }

        private string? FixAvatar (Guid userId)
        {
            return _linkAvatarGenerator(userId);
        }

        public async Task<AttachModel> GetPostContent(Guid postFileId)
        {
            var res = await _context.PostFiles.FirstOrDefaultAsync(x => x.Id == postFileId);

            return _mapper.Map<AttachModel>(res);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
