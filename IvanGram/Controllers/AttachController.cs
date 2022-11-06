using IvanGram.Models;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;

        public AttachController (AttachService attachService)
        {
            _attachService = attachService;
        }

        [HttpPost]
        [Authorize]
        public async Task<List<MetaDataModel>> UploadFiles([FromForm] List<IFormFile> files) => await _attachService.UploadAttaches(files);
    }
}
