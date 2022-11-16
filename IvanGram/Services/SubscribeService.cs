using AutoMapper;
using DAL;
using DAL.Entities;
using IvanGram.Configs;
using IvanGram.Models.Subscribe;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IvanGram.Services
{
    public class SubscribeService : IDisposable
    {
        private readonly UserService _userService;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SubscribeService(DataContext context, UserService userService, IMapper mapper)
        {
            _context = context;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Subscription> GetSubscribeNoteByUsers(User subscribeTo, User follower)
        {
            var dbNote = await _context.Subscriptions.FirstOrDefaultAsync(x => x.SubscribeTo == subscribeTo && x.Follower == follower);
            return dbNote;
        }

        public async Task SubscribeToUser(Guid subscribeToId, Guid followerId)
        {
            if (subscribeToId == followerId)
                throw new Exception("You can`t subscribe yourself");

            var subscribeToUser = await _userService.GetUserById(subscribeToId);
            var followerUser = await _userService.GetUserById(followerId);

            var dbNote = await GetSubscribeNoteByUsers(subscribeToUser, followerUser);

            if (dbNote != null)
            {
                if (dbNote.IsInBlackList)
                    throw new Exception("You are in black list, you cant subscribe");

                if (dbNote.IsActive)
                {
                    if (dbNote.IsAccepted)
                        throw new Exception("You are already subscribed");

                    if (!dbNote.IsAccepted)
                        throw new Exception("User has not accepted your request");
                }
                else
                {
                    dbNote.IsActive = true;
                    await _context.SaveChangesAsync();
                }
            }

            if (dbNote == null)
            {
                var subscription = new Subscription();
                subscription.IsActive = true;
                subscription.SubscribedAt = DateTimeOffset.UtcNow;
                subscription.SubscribeTo = subscribeToUser;
                subscription.Follower = followerUser;
                await _context.AddAsync(subscription);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UnscribeUser(Guid unscribeToId, Guid followerId)
        {
            if (unscribeToId == followerId)
                throw new Exception("You can not unscribe yourself");
            var unscribeToUser = await _userService.GetUserById(unscribeToId);
            var followerUser = await _userService.GetUserById(followerId);
            var dbNote = await GetSubscribeNoteByUsers(unscribeToUser, followerUser);
            if (dbNote == null || !dbNote.IsActive)
                throw new Exception("You are not followed");
            else
            {
                dbNote.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AcceptSubscribe(Guid subscribeToId, Guid followerId)
        {
            if (subscribeToId == followerId)
                throw new Exception("You can`t accept yourself");

            var subscribeToUser = await _userService.GetUserById(subscribeToId);
            var followerUser = await _userService.GetUserById(followerId);

            var dbNote = await GetSubscribeNoteByUsers(subscribeToUser, followerUser);

            if (dbNote == null)
                throw new Exception("Subscription note not found");

            if (dbNote.IsInBlackList)
                throw new Exception("User is in your blacklist");

            if (dbNote.IsActive)
            {
                if (dbNote.IsAccepted)
                    throw new Exception("You are already accepted this user");
                else
                {
                    dbNote.IsAccepted= true;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<UnacceptedSubscribeModel>> GetMyUnacceptedSubscribers (Guid subscribeToId)
        {
            var UnacceptedSubscribersModelList = new List<UnacceptedSubscribeModel>();
            var UnaccepterSubscribersList = _context.Subscriptions.Include(x=>x.Follower)
                .Where(x => x.SubscribeTo.Id == subscribeToId)
                .Where(x => x.IsActive == true)
                .Where(x => x.IsAccepted == false).ToList();
            foreach (var UnaccepterSubscriber in UnaccepterSubscribersList)
            {
                UnacceptedSubscribersModelList.Add(_mapper.Map<UnacceptedSubscribeModel>(UnaccepterSubscriber));
            }
            return UnacceptedSubscribersModelList;
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
