﻿using Common.Consts;
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
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly UserService _userService;

        public PostController(PostService postService, UserService userService)
        {
            _postService = postService;
            _postService.SetLinkGenerator(_linkContentGenerator, _linkAvatarGenerator);
            _userService = userService;
        }

        private string? _linkAvatarGenerator(Guid userId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId,
            });
        }

        private string? _linkContentGenerator(Guid postContentId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId,
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
            => await _postService.GetAllPosts(skip, take);

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
            => await _postService.GetPostComments(PostId);
    }
}
