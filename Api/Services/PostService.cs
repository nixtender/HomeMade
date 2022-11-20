using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Post;
using AutoMapper;
using DAL;
using DAL.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Api.Services
{
    public class PostService
    {
        private readonly DAL.DataContext _context;
        private readonly UserService _userService;
        private readonly IMapper _mapper;

        public PostService (DataContext context, UserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task AddPost(Guid userId, CreatePostModel model, Dictionary<MetadataModel, string> filePaths)
        {
            var user = await _context.Users.Include(x => x.Posts).ThenInclude(y => y.PostPictures).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var dbPost = _mapper.Map<DAL.Entites.Post>(model);
                dbPost.Author = user;

                await _context.Posts.AddAsync(dbPost);
                await _context.SaveChangesAsync();

                if (filePaths.Count > 0)
                {
                    foreach (var filePath in filePaths)
                    {
                        await AddPictureToPost(dbPost, filePath.Key, filePath.Value);
                    }
                }
            }
        }

        private async Task AddPictureToPost(Post post, MetadataModel meta, string filePath)
        {
            //var post = _context.Posts.Include(x => x.PostPictures).FirstOrDefaultAsync(x => x.Id == postId);
            var postPicture = new PostPicture { Author = post.Author, MimeType = meta.MimeType, FilePath = filePath, Name = meta.Name, Size = meta.Size };
            postPicture.Post = post;

            await _context.PostPictures.AddAsync(postPicture);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> GetPost(Guid postId)
        {
            var post = await _context.Posts.Include(x => x.Author).ThenInclude(x => x.Avatar).Include(x => x.PostPictures).Include(x => x.Comments).AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new Exception("post not found");
            return post;
        }

        public async Task<Attach> GetPostPicture(Guid id)
        {
            var atach = await _context.Attaches.FirstOrDefaultAsync(x => x.Id == id);
            return atach;
        }

        public async Task AddComment(Post post, CreateComment model, User user)
        {
            var dbComment = _mapper.Map<Comment>(model);
            dbComment.Author = user.Name;
            dbComment.Post = post;

            await _context.Comments.AddAsync(dbComment);
            await _context.SaveChangesAsync();
        }

        public CommentModel GetComment(Comment comment)
        {
            var commentModel = _mapper.Map<CommentModel>(comment);
            return commentModel;
        }
    }
}
