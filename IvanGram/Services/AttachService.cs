using AutoMapper;
using DAL;
using DAL.Entities;
using IvanGram.Models;
using Microsoft.EntityFrameworkCore;

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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
