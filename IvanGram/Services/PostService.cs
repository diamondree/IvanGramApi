using AutoMapper;
using DAL;
using DAL.Entities;
using IvanGram.Exeptions;
using IvanGram.Models.Attach;
using IvanGram.Models.Post;
using IvanGram.Models.PostComment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace IvanGram.Services
{
    public class PostService : IDisposable
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private Func<Guid, string?>? _linkContentGenerator;
        private Func<Guid, string?>? _linkAvatarGenerator;

        public void SetLinkGenerator(Func<Guid, string?> linkContentGenerator, Func<Guid, string?> linkAvatarGenerator)
        {
            _linkAvatarGenerator = linkAvatarGenerator;
            _linkContentGenerator = linkContentGenerator;
        }

        public PostService (DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PostModel> GetPostByPostId(Guid postId, Guid currentUserId)
        {
            var post = await _context.Posts
                .Include(x => x.Files)
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
                throw new PostNotFoundException();

            await CheckUsersRelationship(post.Author, currentUserId);

            return CreatePostModel(post);
        }
        
        public async Task<List<PostModel>> GetAllPosts (int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Files)
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Skip(skip).Take(take)
                .ToListAsync();
            
            return await GetPostModelList(posts);
        }

        public async Task<List<PostModel>> GetUserPosts (Guid userId, Guid currentUserId)
        {
            var user = await _context.Users
                .Include(x=>x.Posts)
                .ThenInclude(x=>x.Files)
                .AsNoTracking()
                .FirstOrDefaultAsync(x=>x.Id == userId);

            if (user == null)
                throw new UserNotFoundException();

            if (user.Posts == null)
                throw new PostNotFoundException();

            await CheckUsersRelationship(user, currentUserId);

            return await GetPostModelList(user.Posts.ToList());
        }

        public async Task<List<PostModel>> GetUserFolowedUsersPosts (Guid userId)
        {
            var subscribedUsers = await _context.Subscriptions
                .Include(x=>x.Follower)
                .Include(x=>x.SubscribeTo)
                .AsNoTracking()
                .Where(x => x.Follower.Id == userId)
                .Where(x=>x.IsActive == true)
                .Where(x=>x.IsAccepted == true)
                .ToListAsync();

            var posts = new List<PostModel>();

            foreach (var subscribedUser in subscribedUsers)
            {
                var userPosts = await GetUserPosts(subscribedUser.SubscribeTo.Id);
                posts.AddRange(userPosts);
            }

            return posts;
        }

        public async Task CreatePostComment(Guid userId, CreatePostCommentModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == model.PostId);
            if (post == null)
                throw new PostNotFoundException();
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

        public async Task<List<PostCommentModel>> GetPostComments (Guid postId)
        {
            var post = await _context.Posts
                .Include(x => x.Comments).ThenInclude(x => x.Author).ThenInclude(x=>x.Avatar)
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
                throw new PostNotFoundException();
            if (post.Comments == null)
                throw new PostCommentsNotFoundException();
            if (post.Comments.Count <= 0)
                throw new System.Exception("There are not any comments in this post");

            var comments = new List<PostCommentModel>();

            foreach (var comment in post.Comments)
            {
                comments.Add(_mapper.Map<PostComment, PostCommentModel>(comment, opt =>
                opt.AfterMap((src, dest) =>
                {
                    dest.AvatarLink = GetAvatarLink(comment.Author.Id);
                })));
            }

            return comments;
        }

        public async Task<AttachModel> GetPostContent(Guid postFileId)
        {
            var res = await _context.PostFiles.FirstOrDefaultAsync(x => x.Id == postFileId);

            return _mapper.Map<AttachModel>(res);
        }

        public async Task LikePost (Guid postId, Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var dbNote = await _context.PostLikes
                .Where(x => x.PostId == postId)
                .Where(x => x.Author.Id == userId)
                .FirstOrDefaultAsync();

            if (dbNote == null)
            {
                var postLike = new PostLike
                {
                    PostId = postId,
                    UpdatedAt = DateTime.UtcNow,
                    Author = user
                };

                await _context.AddAsync(postLike);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (dbNote.IsActive)
                    throw new System.Exception("You already liked this post");
                else
                {
                    dbNote.IsActive = true;
                    dbNote.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DislikePost (Guid postId, Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var dbNote = await _context.PostLikes
                .Where(x => x.PostId == postId)
                .Where(x => x.Author.Id == userId)
                .FirstOrDefaultAsync();

            if (dbNote == null)
            {
                throw new System.Exception("You dont like this post");
            }
            else
            {
                if (dbNote.IsActive)
                {
                    dbNote.IsActive = false;
                    dbNote.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                else
                    throw new System.Exception("You dont like this post");
            }
        }

        public async Task LikePostComment (Guid postCommentId, Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var dbNote = await _context.CommentLikes
                .Where(x => x.PostCommentId == postCommentId)
                .Where(x => x.Author.Id == userId)
                .FirstOrDefaultAsync();

            if (dbNote == null)
            {
                var postCommentLike = new PostCommentLike
                {
                    PostCommentId = postCommentId,
                    UpdatedAt = DateTime.UtcNow,
                    Author = user
                };

                await _context.AddAsync(postCommentLike);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (dbNote.IsActive)
                    throw new System.Exception("You already liked this comment");
                else
                {
                    dbNote.IsActive = true;
                    dbNote.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DislikePostComment(Guid postCommentId, Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new UserNotFoundException();

            var dbNote = await _context.CommentLikes
                .Where(x => x.PostCommentId == postCommentId)
                .Where(x => x.Author.Id == userId)
                .FirstOrDefaultAsync();

            if (dbNote == null)
                throw new System.Exception("You dont like this post");
            else
            {
                if (dbNote.IsActive)
                {
                    dbNote.IsActive = false;
                    dbNote.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                else
                    throw new System.Exception("You dont like this post");
            }
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

        private Task<List<PostModel>> GetPostModelList(List<Post> posts)
        {
            if (posts == null)
                throw new PostNotFoundException();

            var postModelList = new List<PostModel>();

            foreach (var post in posts)
            {
                postModelList.Add(CreatePostModel(post));
            }

            return Task.FromResult(postModelList);
        }
        private async Task<List<PostModel>> GetUserPosts(Guid userId)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Files).AsNoTracking()
                .Where(x => x.Author.Id == userId)
                .ToListAsync();

            return await GetPostModelList(posts);
        }

        private async Task CheckUsersRelationship(User user, Guid currentUserId)
        {
            var dbNote = await _context.Subscriptions
                .Include(x => x.SubscribeTo)
                .AsNoTracking()
                .Where(x => x.SubscribeTo.Id == user.Id)
                .Where(x => x.Follower.Id == currentUserId)
                .FirstOrDefaultAsync();

            if ((dbNote == null) && user.IsPrivate)
                throw new PostNotFoundException();

            if ((dbNote != null) && (dbNote.IsInBlackList || !dbNote.IsAccepted))
                throw new PostNotFoundException();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
