using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.User;
using DAL.Entites;

namespace Api.Services
{
    public class LinkGeneratorService
    {
        public Func<User, string?>? LinkAvatarGenerator;
        public Func<PostPicture, string?>? LinkPictureGenerator;

        public void FixAvatar(User s, UserModel d)
        {
            d.AvatarLink = s.Avatar == null ? null : LinkAvatarGenerator?.Invoke(s);
        }
        public void FixContent(Post s, PostModel d)
        {
            if (s.PostPictures != null)
            {
                d.Pictures.Clear();
                foreach (var link in s.PostPictures)
                {
                    d.Pictures.Add(LinkPictureGenerator?.Invoke(link));
                }
            }
            else d.Pictures = null;
        }
    }
}
