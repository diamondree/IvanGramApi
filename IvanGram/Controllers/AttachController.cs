using Common.Consts;
using Common.Extensions;
using DAL.Entities;
using IvanGram.Models.Attach;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;
        private readonly UserService _userService;
        private readonly PostService _postService;

        public AttachController (AttachService attachService, UserService userService, PostService postService)
        {
            _attachService = attachService;
            _userService = userService;
            _postService = postService;
        }

        [HttpPost]
        [Authorize]
        public async Task<List<MetaDataModel>> UploadFiles([FromForm] List<IFormFile> files) 
            => await _attachService.UploadAttaches(files);

        [HttpGet]
        [Route("{userId}")]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
            => RenderAttach(await _userService.GetUserAvatar(userId), download);

        [HttpGet]
        public async Task<FileStreamResult> GetCurentUserAvatar(bool download = false)
            => await GetUserAvatar(User.GetClaimValue<Guid>(ClaimNames.Id), download);

        [HttpGet]
        [Route("{postContentId}")]
        public async Task<FileStreamResult> GetPostContent(Guid postContentId, bool download = false)
            => RenderAttach(await _postService.GetPostContent(postContentId), download);

        private FileStreamResult RenderAttach(AttachModel model,bool download)
        {
            var fs = new FileStream(model.FilePath, FileMode.Open);
            var ext = Path.GetExtension(model.Name);
            if (download)
                return File(fs, model.MimeType, $"{model.Id}{ext}");
            else
                return File(fs, model.MimeType);
        }
    }
}
