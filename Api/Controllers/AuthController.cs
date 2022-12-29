using Api.Models.Token;
using Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController (UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> Token(TokenRequestModel model)
        {
            try
            {
                var token = await _userService.GetToken(model.login, model.pass);
                return token;
            }
            catch (Exception)
            {
                throw new HttpRequestException("not authorize", null, statusCode: System.Net.HttpStatusCode.Unauthorized);
            }

        }

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model) 
            => await _userService.GetTokenByRefresh(model.RefreshToken);
    }
}
