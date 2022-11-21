using AutoMapper;
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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AttachService _attachService;

        public UserController(UserService userService, AttachService attachService)
        {
            _userService = userService;
            _userService.SetLinkGenerator(_linkAvatarGenerator);
            _attachService = attachService;
        }

        private string? _linkAvatarGenerator(Guid userId)
        {
            return Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId,
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task RegisterUser(CreateUserModel model) => await _userService.CreateUser(model);

        [HttpGet]
        public async Task<UserModel> GetCurrentUser()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
                return await _userService.GetCurrentUser(userId);
            else
                throw new System.Exception("You are not authorized");
        }

        [HttpPost]
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
                throw new System.Exception("you are not authorized");
        }

        [HttpGet]
        public async Task<FileResult> GetUserAvatar(Guid userId)
        {
            var attach = await _userService.GetUserAvatar(userId);

            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        [HttpPut]
        public async Task SetPrivateProfileSettings(bool SetProfileClosed)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _userService.SetProfilePrivate(userId, SetProfileClosed);
        }

        [HttpGet]
        public async Task<UserModel> GetUserById(Guid userId)
        {
            var currentUserId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _userService.GetUserModelById(currentUserId, userId);
        }

    }
}
