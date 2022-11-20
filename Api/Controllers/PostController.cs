using Api.Models.Attach;
using Api.Models.Comment;
using Api.Models.Post;
using Api.Services;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public PostController(PostService postService, AttachService attachService, UserService userService, IMapper mapper)
        {
            _postService = postService;
            _attachService = attachService;
            _userService = userService;
            _mapper = mapper;
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

        /*[HttpGet]
        public async Task<PostModel> GetPostById(Guid postId)
        {
            var post = await _postService.GetPost(postId);
            List<string> paths = new List<string>();
            if (post.PostPictures != null)
            {
                foreach (var postPicture in post.PostPictures)
                {
                    var path = postPicture.Id;
                    var str = "/Api/post/GetPostPicture?id=";
                    var newPath = str + path;
                    paths.Add(newPath);
                }
            }
            var postModel = new PostModel(paths, post.Description, post.Created, post.Author, post.Comments == null ? 0 : post.Comments.Count);
            return postModel;
        }*/

        [HttpGet]
        public async Task<FileResult> GetPostPicture(Guid id)
        {
            var attach = await _postService.GetPostPicture(id);
            return File(System.IO.File.ReadAllBytes(attach.FilePath), attach.MimeType);
        }

        [HttpPost]
        [Authorize]
        public async Task AddComment(CreateComment model)
        {
            var post = await _postService.GetPost(model.PostId);
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var user = await _userService.GetUserById(userId);
                await _postService.AddComment(post, model, user);
            }    
        }

        [HttpGet]
        public async Task<List<CommentModel>> GetComments(Guid postId)
        {
            var post = await _postService.GetPost(postId);
            List<CommentModel> commentModels = new List<CommentModel>();
            foreach(var comment in post.Comments)
            {
                commentModels.Add(_postService.GetComment(comment));
            }
            return commentModels;
        }
    }
}
