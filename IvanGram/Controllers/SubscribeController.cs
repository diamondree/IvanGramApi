using Common.Consts;
using Common.Extensions;
using IvanGram.Models.Subscribe;
using IvanGram.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IvanGram.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SubscribeController : ControllerBase
    {
        private readonly SubscribeService _subscribeService;

        public SubscribeController(SubscribeService subscribeService)
        {
            _subscribeService = subscribeService;
        }

        [HttpPost]
        public async Task SubsrcibeToUser(Guid subscribeToId)
        {
            var followerId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _subscribeService.SubscribeToUser(subscribeToId, followerId);
        }

        [HttpPost]
        public async Task UnscribeUser(Guid unscribeUserId)
        {
            var followerId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _subscribeService.UnscribeUser(unscribeUserId, followerId);
        }

        [HttpGet]
        public async Task<List<UnacceptedSubscribeModel>> GetMyUnacceptedSubscribers()
        {
            var subscribeToId = User.GetClaimValue<Guid>(ClaimNames.Id);
            return await _subscribeService.GetMyUnacceptedSubscribers(subscribeToId);
        }

        [HttpPut]
        public async Task AcceptUserSubscribe(Guid followerId)
        {
            var subscribeToId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _subscribeService.AcceptSubscribe(subscribeToId, followerId);
        }
    }
}
