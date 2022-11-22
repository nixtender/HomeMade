using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.Post;
using AutoMapper;
using DAL;
using DAL.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public async Task<List<PostModel>> GetPosts()
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Comments)
                .Include(x => x.PostPictures).AsNoTracking().OrderByDescending(x => x.Created)
                .Select(x => _mapper.Map<PostModel>(x))
                .ToListAsync();
            return posts;
        }

        public async Task<PostModel> GetPost(Guid postId)
        {
            var post = await GetPostById(postId);
            return _mapper.Map<PostModel>(post);
        }

        public async Task<Post> GetPostById(Guid postId)
        {
            var post = await _context.Posts.Include(x => x.Author).ThenInclude(x => x.Avatar).Include(x => x.PostPictures).Include(x => x.Comments).Include(x => x.LikePost).AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new Exception("post not found");
            return post;
        }

        public async Task<AttachModel> GetPostPicture(Guid id)
        {
            var atach = await _context.PostPictures.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<AttachModel>(atach);
        }

        public async Task AddComment(Post post, CreateComment model, string userId)
        {
            var dbComment = _mapper.Map<Comment>(model);
            dbComment.AuthorId = userId;

            await _context.Comments.AddAsync(dbComment);
            await _context.SaveChangesAsync();
        }

        public async Task<CommentModel> GetComment(Comment comment)
        {
            var commentModel = _mapper.Map<CommentModel>(comment);
            if (Guid.TryParse(comment.AuthorId, out var authorId))
            {
                var author = await _userService.GetUserById(authorId);
                commentModel.Author = _mapper.Map<Models.User.UserModel>(author);
                return commentModel;
            }
            else throw new Exception("not user");
        }

        public async Task LikeOrNot(Guid postId, Guid userId)
        {
            var post = await _context.Posts.Include(x => x.Author).ThenInclude(x => x.Avatar).Include(x => x.PostPictures).Include(x => x.Comments).Include(x => x.LikePost).FirstOrDefaultAsync(x => x.Id == postId);
            if (post != null)
            {
                if (post.LikePost != null)
                {
                    //var like = post.LikePost;
                    if (post.LikePost.Users.Count > 0)
                    {
                        if (post.LikePost.Users.Contains(userId))
                        {
                            post.LikePost.Users.Remove(userId);
                            post.LikePost.Count--;
                        }
                        else
                        {
                            post.LikePost.Users.Add(userId);
                            post.LikePost.Count++;
                        }
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        post.LikePost.Users.Add(userId);
                        post.LikePost.Count++;
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    List<Guid> users = new List<Guid>();
                    users.Add(userId);
                    var newLike = new LikePost { PostForLike = post, Count = 1, Users = users };
                    post.LikePost = newLike;
                    await _context.SaveChangesAsync();
                }
            }
            else throw new Exception("post not found");
        }
    }
}
