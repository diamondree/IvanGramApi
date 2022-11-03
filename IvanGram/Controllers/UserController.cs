using AutoMapper;
using AutoMapper.QueryableExtensions;
using DAL;
using IvanGram.Models;
using IvanGram.Services;
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
        public async Task<List<UserModel>> GetUsers () => await _userService.GetUsers();

    }
}
