using Api.Models.Attach;
using Api.Models.Post;
using Api.Services;
using AutoMapper;
using DAL.Entites;

namespace Api.Mapper.MapperActions
{
    public class PostPictureMapperAction : IMappingAction<PostPicture, AttachExternalModel>
    {
        private LinkGeneratorService _links;
        public PostPictureMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _links = linkGeneratorService;
        }
        public void Process(PostPicture source, AttachExternalModel destination, ResolutionContext context)
            => _links.FixContent(source, destination);
    }
}
