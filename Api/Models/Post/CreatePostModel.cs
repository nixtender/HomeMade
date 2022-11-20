using Api.Models.Attach;

namespace Api.Models.Post
{
    public class CreatePostModel
    {
        public List<MetadataModel>? Pictures { get; set; }
        public string? Description { get; set; }
    }
}
