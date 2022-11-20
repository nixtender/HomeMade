using Api.Models.User;
using Api.Services;
using AutoMapper;
using DAL.Entites;

namespace Api.Mapper.MapperActions
{
    public class UserAvatarMapperAction : IMappingAction<User, UserModel>
    {
        private LinkGeneratorService _links;
        public UserAvatarMapperAction(LinkGeneratorService linkGeneratorService)
        {
            _links = linkGeneratorService;
        }
        public void Process(User source, UserModel destination, ResolutionContext context) =>
            _links.FixAvatar(source, destination);

    }
}
