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

        public async Task<List<PostModel>> GetPosts(Guid userId, int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Comments)
                .Include(x => x.LikePosts)
                .Include(x => x.PostPictures).AsNoTracking().OrderByDescending(x => x.Created).Skip(skip).Take(take)
                //.Select(x => _mapper.Map<PostModel>(x))
                .ToListAsync();
            List<PostModel> postModels = new List<PostModel>();
            foreach (var post in posts)
            {
                var postModel = _mapper.Map<PostModel>(post);
                foreach (var likePost in post.LikePosts)
                {
                    if (likePost.UserId == userId)
                    {
                        postModel.IsLike = true;
                        break;
                    }
                }
                postModels.Add(postModel);
            }
            return postModels;
        }

        public async Task<List<PostModel>> GetPostsByPublishers(Guid userId, int skip, int take)
        {
            var user = await _userService.GetUserById(userId);
            List<Guid> publisherIds = new List<Guid>();
            if (user.Subscribtions != null)
            {
                foreach (var publisher in user.Subscribtions)
                {
                    publisherIds.Add(publisher.PublisherId);
                }
            }
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Comments)
                .Include(x => x.LikePosts)
                .Include(x => x.PostPictures)
                .Where(x => publisherIds.Contains(x.AuthorId) || x.AuthorId == userId).AsNoTracking().OrderByDescending(x => x.Created).Skip(skip).Take(take)
                .ToListAsync();
            List<PostModel> postModels = new List<PostModel>();
            foreach (var post in posts)
            {
                var postModel = _mapper.Map<PostModel>(post);
                foreach (var likePost in post.LikePosts)
                {
                    if (likePost.UserId == userId)
                    {
                        postModel.IsLike = true;
                        break;
                    }
                }
                postModels.Add(postModel);
            }
            return postModels;
        }

        public async Task<List<PostModel>> GetPostsByMe(Guid userId, Guid selectUserId, int skip, int take)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.Comments)
                .Include(x => x.LikePosts)
                .Include(x => x.PostPictures)
                .Where(x => x.AuthorId == selectUserId).AsNoTracking().OrderByDescending(x => x.Created).Skip(skip).Take(take)
                .ToListAsync();
            List<PostModel> postModels = new List<PostModel>();
            foreach (var post in posts)
            {
                var postModel = _mapper.Map<PostModel>(post);
                foreach (var likePost in post.LikePosts)
                {
                    if (likePost.UserId == userId)
                    {
                        postModel.IsLike = true;
                        break;
                    }
                }
                postModels.Add(postModel);
            }
            return postModels;
        }

        public async Task<PostModel> GetPost(Guid postId)
        {
            var post = await GetPostById(postId);
            return _mapper.Map<PostModel>(post);
        }

        public async Task<Post> GetPostById(Guid postId)
        {
            var post = await _context.Posts.Include(x => x.Author).ThenInclude(x => x.Avatar).Include(x => x.PostPictures).Include(x => x.Comments).ThenInclude(x => x.LikeComments).Include(x => x.LikePosts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new Exception("post not found");
            return post;
        }

        public async Task<AttachModel> GetPostPicture(Guid id)
        {
            var atach = await _context.PostPictures.FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<AttachModel>(atach);
        }

        public async Task DeletePost(Guid postId)
        {
            var post = await GetPostById(postId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
        }

        public async Task AddComment(Post post, CreateComment model, string userId)
        {
            var dbComment = _mapper.Map<Comment>(model);
            dbComment.AuthorId = userId;

            await _context.Comments.AddAsync(dbComment);
            await _context.SaveChangesAsync();
        }

        public async Task<CommentModel> GetComment(Comment comment, Guid? userId)
        {
            var commentModel = _mapper.Map<CommentModel>(comment);
            if (Guid.TryParse(comment.AuthorId, out var authorId))
            {
                var author = await _userService.GetUserById(authorId);
                commentModel.Author = _mapper.Map<Models.User.UserModel>(author);

                if (comment.LikeComments.Any(x => x.UserId == userId))
                {
                    commentModel.IsLiked = true;
                }

                return commentModel;
            }
            else throw new Exception("not user");
        }

        public async Task<Comment> GetCommentById(Guid commentId)
        {
            var comment = await _context.Comments.Include(x => x.Post).Include(x => x.LikeComments).FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment == null)
                throw new Exception("comment not found");
            return comment;
        }

        public async Task LikeOrNotPost(CreateLikeModel model, Post post)
        {
            if (await _context.LikePosts.AnyAsync(x => x.UserId == model.UserId && x.PostId == model.ObjectId))
            {
                _context.LikePosts.Remove(post.LikePosts.FirstOrDefault(x => x.UserId == model.UserId && x.PostId == model.ObjectId));
                _context.SaveChanges();
                return;
            }
            var likePost = _mapper.Map<LikePost>(model);
            await _context.LikePosts.AddAsync(likePost);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LikePostModel>> GetLikePosts(Guid meUserId)
        {
            var likePosts = await _context.LikePosts.AsNoTracking().Include(x => x.User).ThenInclude(x => x.Avatar).Include(x => x.Post).ThenInclude(x => x.PostPictures).Include(x => x.Post).ThenInclude(x => x.Author).ThenInclude(x => x.Avatar).Where(x => x.Post.AuthorId == meUserId).ToListAsync();
            List<LikePostModel> list = new List<LikePostModel>();
            if (likePosts != null)
            {
                foreach(var likePost in likePosts)
                {
                    list.Add(_mapper.Map<LikePostModel>(likePost));
                }
            }
            return list;
        }

        public async Task LikeOrNotComment(CreateLikeModel model, Comment comment)
        {
            if (await _context.LikeComments.AnyAsync(x => x.UserId == model.UserId && x.CommentId == model.ObjectId))
            {
                _context.LikeComments.Remove(comment.LikeComments.FirstOrDefault(x => x.UserId == model.UserId && x.CommentId == model.ObjectId));
                _context.SaveChanges();
                return;
            }
            var likeComment = _mapper.Map<LikeComment>(model);
            await _context.LikeComments.AddAsync(likeComment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LikeCommentModel>> GetLikeComments(Guid meUserId)
        {
            var likeComments = await _context.LikeComments.AsNoTracking().Include(x => x.User).ThenInclude(x => x.Avatar).Include(x => x.Comment).Where(x => Guid.Parse(x.Comment.AuthorId) == meUserId).ToListAsync();
            List<LikeCommentModel> list = new List<LikeCommentModel>();
            if (likeComments != null)
            {
                foreach (var likeComment in likeComments)
                {
                    list.Add(_mapper.Map<LikeCommentModel>(likeComment));
                }
            }
            return list;
        }
    }
}
