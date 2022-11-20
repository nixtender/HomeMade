using Api.Models.Attach;
using Api.Models.Post;
using Api.Services;
using AutoMapper;
using DAL.Entites;

namespace Api.Mapper.MapperActions
{
    public class PostPictureMapperAction : IMappingAction<Post, PostModel>
    {
        private LinkGeneratorService _links;
        public PostPictureMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _links = linkGeneratorService;
        }
        public void Process(Post source, PostModel destination, ResolutionContext context)
            => _links.FixContent(source, destination);
    }
}
