using IvanGram.Models.Token;
using IvanGram.Services;
using Microsoft.AspNetCore.Mvc;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> GetTokens (TokenRequestModel model) 
            => await _userService.GetTokens(model.Login, model.Password);

        [HttpPost]
        public async Task<TokenModel> GetTokensByRefreshToken(RefreshTokenRequestModel model)
            => await _userService.GetTokensByRefreshToken(model.RefreshToken);

    }
}
