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
using System.ComponentModel.Design;

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
                .Include(x => x.PostPictures).Where(x => x.IsExist).AsNoTracking().OrderByDescending(x => x.Created)
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
            var post = await _context.Posts.Include(x => x.Author).ThenInclude(x => x.Avatar).Include(x => x.PostPictures).Include(x => x.Comments).Include(x => x.LikePosts).AsNoTracking().FirstOrDefaultAsync(x => x.Id == postId);
            if (post == null)
                throw new Exception("post not found");
            return post;
        }

        public async Task DeletePost(Guid postId, Guid userId)
        {
            var post = await GetPostById(postId);
            if (post.AuthorId == userId)
            {
                post.IsExist = false;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
            else throw new Exception("someone else's post");
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

        public async Task<CommentModel> GetComment(Guid commentId)
        {
            var comment = await GetCommentById(commentId);
            var commentModel = _mapper.Map<CommentModel>(comment);
            if (Guid.TryParse(comment.AuthorId, out var authorId))
            {
                var author = await _userService.GetUserById(authorId);
                commentModel.Author = _mapper.Map<Models.User.UserModel>(author);
                return commentModel;
            }
            else throw new Exception("not user");
        }

        public async Task<Comment> GetCommentById(Guid commentId)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment != null)
                return comment;
            else throw new Exception("comment not found");
        }

        public async Task DeleteComment(Guid commentId, Guid userId)
        {
            var comment = await GetCommentById(commentId);
            if (comment.AuthorId == userId.ToString())
            {
                comment.IsExist = false;
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();
            }
            else throw new Exception("someone else's post");
        }

        public async Task LikeOrNotPost(Guid postId, Guid userId)
        {
            var post = await _context.Posts.Include(x => x.Author).ThenInclude(x => x.Avatar).Include(x => x.PostPictures).Include(x => x.Comments).Include(x => x.LikePosts).FirstOrDefaultAsync(x => x.Id == postId);
            if (post != null)
            {
                var user = await _context.Users.Include(x => x.Likes).AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
                foreach (var likePost in post.LikePosts)
                {
                    if (likePost.UserLikePostId == userId)
                    {
                        _context.Likes.Remove(likePost);
                        _context.SaveChanges();
                        return;
                    }
                }
                var newLikePost = new LikePost { UserLike = user, Post = post };
                user.Likes.Add(newLikePost);
                post.LikePosts.Add(newLikePost);
                await _context.SaveChangesAsync();
            }
            else throw new Exception("post not found");
        }

        public async Task LikeOrNotComment(Guid commentId, Guid userId)
        {
            var comment = await _context.Comments.Include(x => x.LikeComments).FirstOrDefaultAsync(x => x.Id == commentId); 
            if (comment != null)
            {
                var user = await _context.Users.Include(x => x.Likes).AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
                foreach (var likeComment in comment.LikeComments)
                {
                    if (likeComment.UserLikeCommentId == userId)
                    {
                        _context.Likes.Remove(likeComment);
                        _context.SaveChanges();
                        return;
                    }
                }
                var newLikeComment = new LikeComment { UserLike = user, Comment = comment };
                user.Likes.Add(newLikeComment);
                comment.LikeComments.Add(newLikeComment);
                await _context.SaveChangesAsync();
            }
            else throw new Exception("post not found");
        }

        /*public async Task LikeOrNotComment(Guid commentId, Guid userId)
        {
            var comment = await _context.Comments.Include(x => x.LikeComments).FirstOrDefaultAsync(x => x.Id == commentId);
            if (comment != null)
            {
                if (comment.LikeComment != null)
                {
                    LikeOrNot(comment.LikeComment, userId);
                    await _context.SaveChangesAsync();
                    *//*if (post.LikePost.Users.Count > 0)
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
                    }*//*

                }
                else
                {
                    List<Guid> users = new List<Guid>();
                    users.Add(userId);
                    var newLike = new LikeComment { CommentForLike = comment, Count = 1, Users = users };
                    comment.LikeComment = newLike;
                    await _context.SaveChangesAsync();
                }
            }
            else throw new Exception("comment not found");
        }
    
        private void LikeOrNot(Like like, Guid userId)
        {
            if (like.Users.Count > 0)
            {
                if (like.Users.Contains(userId))
                {
                    like.Users.Remove(userId);
                    like.Count--;
                }
                else
                {
                    like.Users.Add(userId);
                    like.Count++;
                }
            }
            else
            {
                like.Users.Add(userId);
                like.Count++;
            }
        }*/
    }
}
