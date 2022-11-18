using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Consts;
using DAL;
using DAL.Entities;
using IvanGram.Configs;
using IvanGram.Exeptions;
using IvanGram.Models.Attach;
using IvanGram.Models.Post;
using IvanGram.Models.Token;
using IvanGram.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IvanGram.Services
{
    public class UserService : IDisposable
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly AuthConfig _config;
        private readonly SessionService _session;
        private readonly AttachService _attachService;

        public UserService(IMapper mapper, DataContext context, IOptions<AuthConfig> config, SessionService session, AttachService attachService)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
            _session = session;
            _attachService = attachService;
        }

        private async Task<bool> CheckUserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            if (await CheckUserExists(model.Email))
                throw new System.Exception("User exists");
            var DBUser = _mapper.Map<User>(model);
            var temp = await _context.Users.AddAsync(DBUser);
            await _context.SaveChangesAsync();
            return temp.Entity.Id;
        }

        public async Task<User> GetUserByCredention (string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email.ToLower() == login.ToLower());
            if (user == null)
                throw new UserNotFoundException();

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new System.Exception("Password is incorrect");

            return user;
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x=>x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new UserNotFoundException();
            return user;
        }

        public async Task<UserModel> GetUserModelById(Guid currentUserId, Guid userId)
        {
            var user = await GetUserById(userId);
            var dbNote = await _context.Subscriptions
                .Include(x=>x.SubscribeTo)
                .Include(x=>x.Follower)
                .Where(x => x.SubscribeTo.Id == userId)
                .Where(x => x.Follower.Id == currentUserId)
                .FirstOrDefaultAsync();
            if ((dbNote != null) && dbNote.IsInBlackList)
            {
                throw new UserNotFoundException();
            }
            return _mapper.Map<UserModel>(user);
        }

        public async Task<UserModel> GetCurrentUser(Guid id)
        {
            var user = await GetUserById(id);
            return _mapper.Map<UserModel>(user);
        }

        private TokenModel GenerateTokens(UserSession userSession)
        {
            if (userSession.User == null)
                throw new System.Exception("You are wizard. This exception cant be throwed. Go and check your magic abilities!!! ^_^");

            var DTNow = DateTime.Now;

            var acessToken = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: DTNow,
                claims: new Claim[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userSession.User.Name),
                    new Claim(ClaimNames.SessionId, userSession.Id.ToString()),
                    new Claim(ClaimNames.Id, userSession.User.Id.ToString())
                },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedAcessToken = new JwtSecurityTokenHandler().WriteToken(acessToken);

            var refreshToken = new JwtSecurityToken(
                notBefore: DTNow,
                claims: new Claim[]
                {
                    new Claim(ClaimNames.RefreshToken, userSession.RefreshToken.ToString())
                },
                expires: DateTime.Now.AddHours(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            return new TokenModel(encodedAcessToken, encodedRefreshToken);
        }

        public async Task<TokenModel> GetTokensByRefreshToken(string refreshToken)
        {
            var validParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _config.SymmetricSecurityKey()
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            if (principal.Claims.FirstOrDefault(x => x.Type == ClaimNames.RefreshToken)?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshId))
            {
                var session = await _session.GetSessionByRefreshToken(refreshId);

                if (!session.IsActive)
                    throw new System.Exception("Session does not active");

                session.RefreshToken = Guid.NewGuid();

                return GenerateTokens(session);
            }
            else
                throw new SecurityTokenException("Invalid token");
        }

        public async Task<TokenModel> GetTokens(string login, string password)
        {
            var user = await GetUserByCredention(login, password);

            var userSession = await _context.UserSessions.AddAsync(new DAL.Entities.UserSession
            {
                User = user,
                RefreshToken = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Id = Guid.NewGuid()
            });

            await _context.SaveChangesAsync();

            return GenerateTokens(userSession.Entity);
        }

        public async Task UploadUserAvatar(AddUserAvatarModel model)
        {
            var user = await _context.Users.Include(x=>x.Avatar).FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user != null )
            {
                var avatar = new UserAvatar
                {
                    Author = user,
                    MimeType = model.MetaDataModel.MimeType,
                    FilePath = model.FilePath,
                    Name = model.MetaDataModel.Name,
                    Size = model.MetaDataModel.Size
                };
                user.Avatar = avatar;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AttachModel> GetUserAvatar (Guid userId)
        {
            var user = await GetUserById(userId);
            var attach = _mapper.Map<AttachModel>(user.Avatar);
            if (attach == null)
                throw new System.Exception("User does not have avatar");
            return attach;
        }

        public async Task CreateUserPost(AddPostModel model, Guid UserId)
        {
            var user = await _context.Users.Include(x=>x.Posts).FirstOrDefaultAsync(x => x.Id == UserId);

            if (user != null)
            {
                var tempPostFileList = new List<PostFile>();
                if (model.Files.Count > 0)
                {
                    
                    foreach (var file in model.Files)
                    {
                        var filePath = _attachService.CopyFileToAttaches(file);

                        var postFile = new PostFile
                        {
                            Name = file.Name,
                            MimeType = file.MimeType,
                            FilePath = filePath,
                            Size = file.Size,
                            Author = user
                        };
                        tempPostFileList.Add(postFile);
                    }
                }
                else
                {
                    throw new Exeptions.FileNotFoundException();
                }
                var post = new Post
                {
                    Author = user,
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTimeOffset.UtcNow,
                    Description = model.Description,
                    Files = tempPostFileList
                };

                await _context.Posts.AddAsync(post);

                
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetProfilePrivate (Guid userId, bool setProfileClosed)
        {
            var user = await GetUserById(userId);
            if (user.IsPrivate == setProfileClosed)
                throw new System.Exception($"Your profile setting <<IsPrivate>> is already {setProfileClosed}");
            user.IsPrivate = setProfileClosed;
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
