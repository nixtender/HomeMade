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
            CreateMap<DAL.Entites.Post, Models.Post.PostModel>()
                .ForMember(d => d.Pictures, m => m.MapFrom(s => s.PostPictures))
                .ForMember(d => d.CreatedPost, m => m.MapFrom(s => s.Created))
                .ForMember(d => d.CommentCount, m => m.MapFrom(s => s.Comments == null ? 0 : s.Comments.Count))
                .AfterMap<PostPictureMapperAction>();

            CreateMap<CreateComment, DAL.Entites.Comment>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<DAL.Entites.Comment, CommentModel>();

            CreateMap<Models.Like.LikeModel, DAL.Entites.LikePost>();
        }
    }
}
