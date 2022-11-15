﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Consts;
using Common.Extensions;
using DAL;
using IvanGram.Models;
using IvanGram.Models.Attach;
using IvanGram.Models.Post;
using IvanGram.Models.User;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AttachService _attachService;

        public UserController(UserService userService, AttachService attachService)
        {
            _userService = userService;
            _attachService = attachService;
        }

        [HttpPost]
        public async Task RegisterUser(CreateUserModel model) => await _userService.CreateUser(model);

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
                return await _userService.GetUser(userId);
            else
                throw new Exception("You are not authorized");
        }

        [HttpPost]
        [Authorize]
        public async Task UploadAvatar(MetaDataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                var path = _attachService.CopyFileToAttaches(model);

                var addUserAvatarModel = new AddUserAvatarModel
                {
                    MetaDataModel = model,
                    UserId = userId,
                    FilePath = path
                };

                await _userService.UploadUserAvatar(addUserAvatarModel);
            }
            else
                throw new Exception("you are not authorized");
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid userId)
        {
            var attach = await _userService.GetUserAvatar(userId);

            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        [HttpPost]
        [Authorize]
        public async Task SubsrcibeToUser(Guid subscribeToId)
        {
            var followerId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _userService.SubscribeToUser(subscribeToId, followerId);
        }

        [HttpPost]
        [Authorize]
        public async Task UnscribeUser(Guid unscribeUserId)
        {
            var followerId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _userService.UnscribeUser(unscribeUserId, followerId);
        }
    }
}
