using AutoMapper;
using DAL.Entities;
using IvanGram.Models.Attach;
using IvanGram.Models.Post;
using IvanGram.Services;

namespace IvanGram.Mapper.MapperAction
{
    public class PostAttachMapperAction : IMappingAction<PostFile, AttachExternalModel>
    {
        private LinkGeneratorService _links;
        public PostAttachMapperAction(LinkGeneratorService links)
        {
            _links = links;
        }

        public void Process(PostFile source, AttachExternalModel destination, ResolutionContext context)
             => _links.FixPostContent(source, destination);
    }
}
