using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using IvanGram.Models;
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
        public async Task CreateUser(CreateUserModel model) => await _userService.CreateUser(model);

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers() => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdStr = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
                return await _userService.GetUser(userId);
            else
                throw new Exception("You are not authorized");
        }

        [HttpPost]
        [Authorize]
        public async Task AddAvatarToUser(MetaDataModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var path = _attachService.CopyFileToAttaches(model);

                    var addUserAvatarModel = new AddUserAvatarModel
                    {
                        MetaDataModel = model,
                        UserId = userId,
                        FilePath = path
                    };

                    await _userService.AddAvatarToUser(addUserAvatarModel);
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
        public async Task AddPostToUser(AddPostModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var addPostRequestModel = new AddPostRequestModel { UserId = userId, PostId = Guid.NewGuid() };
                if (model.Description != null)
                    addPostRequestModel.Descriprion = model.Description;
                foreach (var file in model.Files)
                {
                    _attachService.CopyFileToAttaches(file);
                    addPostRequestModel.Files.Add(file);
                }
                
                if (addPostRequestModel.Files != null)
                    await _userService.CreateUserPost(addPostRequestModel);
                else
                    throw new Exception("Files have not been uploaded");

            }
            else
                throw new Exception("You are not authorized");
        }
    }
}
