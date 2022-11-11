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
                .Include(x => x.Files)
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
                throw new Exception("Post not found");

            return CreatePostModel(post);
        }
        
        public async Task<List<PostModel>> GetPosts(int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Files)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip).Take(take)
                .ToListAsync();

            if (posts == null)
                throw new Exception("Posts not found");

            var postModelList = new List<PostModel>();

            foreach (var post in posts)
            {
                postModelList.Add(CreatePostModel(post));
            }
            return postModelList;
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
                    opt.AfterMap((src, dest) => dest.AvatarLink = GetAvatarLink(src.AuthorId));
                }));
            }
            return commentsWithAvatarLink;
        }

        public async Task<AttachModel> GetPostContent(Guid postFileId)
        {
            var res = await _context.PostFiles.FirstOrDefaultAsync(x => x.Id == postFileId);

            return _mapper.Map<AttachModel>(res);
        }


        private string? GetAvatarLink(Guid userId)
            => _linkAvatarGenerator(userId);

        private List<string>? GetAttachLink(List<AttachModel> models)
        {
            var res = new List<string>();
            foreach (var model in models)
            {
                res.Add(_linkContentGenerator(model.Id));
            }
            return res;
        }

        private PostModel CreatePostModel(Post post)
        {
            var postAttachModelsList = _mapper.Map<List<AttachModel>>(post.Files);

            var postModel = _mapper.Map<Post, PostModel>(post, opt =>
            opt.AfterMap((src, dest) =>
            {
                dest.AuthorAvatar = GetAvatarLink(post.Author.Id);
                dest.AttachesLinks = GetAttachLink(postAttachModelsList);
            }));
            return postModel;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
