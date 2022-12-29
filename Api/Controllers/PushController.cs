using Api.Models.Push;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PushController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly GooglePushService _googlePushService;

        public PushController(UserService userService, GooglePushService googlePushService)
        {
            _userService = userService;
            _googlePushService = googlePushService;
        }

        [HttpPost]
        public async Task Subscribe(PushTokenModel model)
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _userService.SetPushToken(userId, model.Token);
            }
        }

        [HttpDelete]
        public async Task UnSubscribe()
        {
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _userService.SetPushToken(userId);
            }
        }

        [HttpPost]
        public async Task<List<string>> SendPush(SendPushModel model)
        {
            var res = new List<string>();
            var userIdString = User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                var userIdT = model.UserId ?? userId;
                var token = await _userService.GetPushToken(userIdT);
                if (token != default)
                {
                    res =  _googlePushService.SendNotification(token, model.Push);
                }
            }
            return res;
        }
    }
}
