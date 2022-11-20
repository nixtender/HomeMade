using Api.Mapper.MapperActions;
using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Post;
using Api.Models.User;
using AutoMapper;
using Common;

namespace Api.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, DAL.Entites.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));
            CreateMap<DAL.Entites.User, UserModel>().AfterMap<UserAvatarMapperAction>();

            CreateMap<DAL.Entites.Attach, AttachModel>();

            CreateMap<CreatePostModel, DAL.Entites.Post>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));

            CreateMap<CreateComment, DAL.Entites.Comment>().ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<DAL.Entites.Comment, CommentModel>();
        }
    }
}
