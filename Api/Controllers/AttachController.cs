using Api.Models.Attach;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachController : ControllerBase
    {
        private readonly AttachService _attachService;
        private readonly UserService _userService;
        private readonly PostService _postService;

        public AttachController(AttachService attachService, UserService userService, PostService postService)
        {
            _attachService = attachService;
            _userService = userService;
            _postService = postService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files) => await _attachService.UploadFiles(files);

        [HttpGet]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
            => RenderAttach(await _userService.GetUserAvatar(userId), download);

        [HttpGet]
        public async Task<FileStreamResult> GetCurentUserAvatar(bool download = false)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                return await GetUserAvatar(userId, download);
            }
            else throw new Exception("error");
        }

        [HttpGet]
        public async Task<FileStreamResult> GetPostPicture(Guid postPictureId, bool download = false)
            => RenderAttach(await _postService.GetPostPicture(postPictureId), download);

        private FileStreamResult RenderAttach(AttachModel attach, bool download)
        {
            var fs = new FileStream(attach.FilePath, FileMode.Open);
            var ext = Path.GetExtension(attach.Name);
            if (download)
                return File(fs, attach.MimeType, $"{attach.Id}{ext}");
            else
                return File(fs, attach.MimeType);

        }
    }
}
