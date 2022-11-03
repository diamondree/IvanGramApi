using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using IvanGram.Models;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController (UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task CreateUser (CreateUserModel model) => await _userService.CreateUser(model);

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers () => await _userService.GetUsers();

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            var userIdStr = User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
                return await _userService.GetUser(userId);
            else
                throw new Exception("You are not authorized");
        }
    }
}
