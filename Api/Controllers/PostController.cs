using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.Post;
using Api.Services;
using AutoMapper;
using Common.Consts;
using Common.Extentions;
using DAL.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly AttachService _attachService;
        private readonly UserService _userService;

        public PostController(PostService postService, AttachService attachService, UserService userService, LinkGeneratorService links)
        {
            _postService = postService;
            _attachService = attachService;
            _userService = userService;
            links.LinkPictureGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostPicture), new
            {
                postPictureId = x.Id,
            });
            links.LinkAvatarGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task AddPost(CreatePostModel post)
        {
            List<MetadataModel>? pictures = post.Pictures;
            
            Dictionary<MetadataModel, string> paths = new Dictionary<MetadataModel, string>();
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                if (pictures != null)
                {
                    foreach (var picture in pictures)
                    {
                        var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), picture.TempId.ToString()));
                        if (!tempFi.Exists)
                            throw new Exception("file not found");
                        else
                        {
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "attaches", picture.TempId.ToString());
                            paths.Add(picture, path);
                            var destFi = new FileInfo(path);
                            if (destFi.Directory != null && !destFi.Directory.Exists)
                                destFi.Directory.Create();
                            System.IO.File.Copy(tempFi.FullName, path, true);

                        }
                    }
                }
                await _postService.AddPost(userId, post, paths);
            }
            else throw new Exception("you are not authorized");
            
        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10) => await _postService.GetPosts(User.GetClaimValue<Guid>(ClaimNames.Id), skip, take);

        [HttpGet]
        public async Task<List<PostModel>> GetPostsByPublishers(int skip = 0, int take = 10) => await _postService.GetPostsByPublishers(User.GetClaimValue<Guid>(ClaimNames.Id), skip, take);

        [HttpGet]
        public async Task<List<PostModel>> GetPostsByMe(Guid userId, int skip = 0, int take = 10) => await _postService.GetPostsByMe(User.GetClaimValue<Guid>(ClaimNames.Id), userId, skip, take);

        [HttpGet]
        public async Task<PostModel> GetPost(Guid postId) => await _postService.GetPost(postId);

        [HttpDelete]
        public async Task DeletePost(Guid postId) => await _postService.DeletePost(postId);

        [HttpPost]
        [Authorize]
        public async Task AddComment(CreateComment model)
        {
            var post = await _postService.GetPostById(model.PostId);
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            await _postService.AddComment(post, model, userIdString); 
        }

        [HttpGet]
        public async Task<List<CommentModel>> GetComments(Guid postId)
        {
            var post = await _postService.GetPostById(postId);
            List<CommentModel> commentModels = new List<CommentModel>();
            if (post.Comments != null)
            {
                foreach (var comment in post.Comments)
                {
                    commentModels.Add(await _postService.GetComment(comment, User.GetClaimValue<Guid>(ClaimNames.Id)));
                }
            }
            else throw new Exception("No Comments");
            return commentModels;
        }

        [HttpPost]
        [Authorize]
        public async Task LikeOrNotPost(CreateLikeModel model)
        {
            var post = await _postService.GetPostById(model.ObjectId);
            var currentUserIdStr = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(currentUserIdStr, out var currentUserId))
            {
                model.UserId = currentUserId;
                await _postService.LikeOrNotPost(model, post);
            }
            else throw new Exception("you are not authorized");
        }

        /*[HttpPost]
        [Authorize]
        public async Task<List<LikeModel>> GetLikePosts()
        {

        }*/

        [HttpPost]
        [Authorize]
        public async Task LikeOrNotComment(CreateLikeModel model)
        {
            var comment = await _postService.GetCommentById(model.ObjectId);
            var currentUserIdStr = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(currentUserIdStr, out var currentUserId))
            {
                model.UserId = currentUserId;
                await _postService.LikeOrNotComment(model, comment);
            }
            else throw new Exception("you are not authorized");
        }
    }
}
