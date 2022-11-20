using Api.Models.Attach;
using Api.Models.User;
using DAL.Entites;

namespace Api.Services
{
    public class LinkGeneratorService
    {
        public Func<User, string?>? LinkAvatarGenerator;

        public void FixAvatar(User s, UserModel d)
        {
            d.AvatarLink = s.Avatar == null ? null : LinkAvatarGenerator?.Invoke(s);
        }
    }
}
