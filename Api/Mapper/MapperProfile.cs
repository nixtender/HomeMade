using Api.Mapper.MapperActions;
using Api.Models.Attach;
using Api.Models.Chat;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.Post;
using Api.Models.Subscribtion;
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
            CreateMap<DAL.Entites.User, UserModel>()
                .ForMember(d => d.PostCount, m => m.MapFrom(s => s.Posts == null ? 0 : s.Posts.Count))
                .ForMember(d => d.PublisherCount, m => m.MapFrom(s => s.Subscribtions == null ? 0 : s.Subscribtions.Count))
                .ForMember(d => d.FollowerCount, m => m.MapFrom(s => s.Followers == null ? 0 : s.Followers.Count))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<DAL.Entites.Attach, AttachModel>();

            CreateMap<CreatePostModel, DAL.Entites.Post>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<DAL.Entites.Post, Models.Post.PostModel>()
                .ForMember(d => d.Pictures, m => m.MapFrom(s => s.PostPictures))
                .ForMember(d => d.CreatedPost, m => m.MapFrom(s => s.Created))
                .ForMember(d => d.CommentCount, m => m.MapFrom(s => s.Comments == null ? 0 : s.Comments.Count))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.LikePosts == null ? 0 : s.LikePosts.Count));
            //.AfterMap<PostPictureMapperAction>();
            CreateMap<DAL.Entites.PostPicture, AttachExternalModel>().AfterMap<PostPictureMapperAction>();

            CreateMap<CreateComment, DAL.Entites.Comment>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<DAL.Entites.Comment, CommentModel>()
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.LikeComments == null ? 0 : s.LikeComments.Count));

            CreateMap<CreateLikeModel, DAL.Entites.LikePost>()
                .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.PostId, m => m.MapFrom(s => s.ObjectId));
            CreateMap<DAL.Entites.LikePost, LikeModel>()
                .ForMember(d => d.ObjectId, m => m.MapFrom(s => s.PostId));
            CreateMap<CreateLikeModel, DAL.Entites.LikeComment>()
                .ForMember(d => d.CreateDate, m => m.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.CommentId, m => m.MapFrom(s => s.ObjectId));

            CreateMap<CreateSubscribtionModel, DAL.Entites.Subscribtion>()
                .ForMember(d => d.SubscriptionDate, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<DAL.Entites.Subscribtion, SubscribtionModel>();

            CreateMap<DAL.Entites.Chat, ChatModel>()
                .ForMember(d => d.EndMessage, m => m.MapFrom(s => s.Messages.MaxBy(y => y.SendingTime).Text));
            CreateMap<CreateMessageModel, DAL.Entites.Message>()
                .ForMember(d => d.SendingTime, m => m.MapFrom(s => DateTime.UtcNow));
            CreateMap<DAL.Entites.Message, MessageModel>();
        }
    }
}
