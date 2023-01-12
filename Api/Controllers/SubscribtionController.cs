using Api.Models.Subscribtion;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SubscribtionController : ControllerBase
    {
        private readonly SubscribtionService _subscribtionService;

        public SubscribtionController(SubscribtionService subscribtionService, LinkGeneratorService links)
        {
            _subscribtionService = subscribtionService;

            links.LinkAvatarGenerator = x =>
            Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }

        [HttpPost]
        public async Task SubscribeToUser(CreateSubscribtionModel model)
        {
            var followerId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (followerId != default)
            {
                model.FollowerId = followerId;
                await _subscribtionService.SubscribeToUser(model);
            }
            else throw new Exception("user not found");
        }

        [HttpDelete]
        public async Task UnSubscribeFromUser(CreateSubscribtionModel model)
        {
            var followerId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (followerId != default)
            {
                model.FollowerId = followerId;
                await _subscribtionService.UnSubscribeFromUser(model);
            }
            else throw new Exception("user not found");
        }

        [HttpGet]
        public async Task<IEnumerable<UserModel>> ShowSubsribes(Guid userId)
            => await _subscribtionService.GetSubscribtions(userId);

        [HttpGet]
        public async Task<IEnumerable<UserModel>> ShowFollowers(Guid userId)
            => await _subscribtionService.GetFollowers(userId);

        [HttpGet]
        public async Task<List<SubscribtionModel>> GetSubAndFol()
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _subscribtionService.GetSubAndFol(userId);
        }
    }
}
