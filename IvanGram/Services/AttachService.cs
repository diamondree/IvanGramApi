using AutoMapper;
using DAL;
using DAL.Entities;
using IvanGram.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IvanGram.Services
{
    public class AttachService : IDisposable
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AttachService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MetaDataModel> UploadAttach(IFormFile file)
        {
            var meta = new MetaDataModel
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length,
            };
            var newPath = Path.Combine(Path.GetTempPath(), meta.TempId.ToString());

            var fileinfo = new FileInfo(newPath);
            if (fileinfo.Exists)
            {
                throw new Exception("file exist");
            }
            else
            {
                if (fileinfo.Directory == null)
                {
                    throw new Exception("temp is null");
                }
                else
                if (!fileinfo.Directory.Exists)
                {
                    fileinfo.Directory?.Create();
                }

                using (var stream = System.IO.File.Create(newPath))
                {
                    await file.CopyToAsync(stream);
                }

                return meta;
            }
        }

        public async Task<List<MetaDataModel>> UploadAttaches(List<IFormFile> files)
        {
            var res = new List<MetaDataModel>();
            foreach (var file in files)
            {
                res.Add(await UploadAttach(file));
            }
            return res;
        }

        public async Task<AttachModel> GetAvtarFromUser(User user) => _mapper.Map<AttachModel>(user.Avatar);

        public async Task<List<AttachModel>> GetPostAttaches(Post post) => _mapper.Map<List<AttachModel>>(post.Files);

        public string CopyFileToAttaches(MetaDataModel model)
        {
            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
            if (!tempFi.Exists)
                throw new Exception("file not found");
            else
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", model.TempId.ToString());
                var destFi = new FileInfo(path);
                if (destFi.Directory != null && !destFi.Directory.Exists)
                    destFi.Directory.Create();

                File.Copy(tempFi.FullName, path, true);

                return path;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
