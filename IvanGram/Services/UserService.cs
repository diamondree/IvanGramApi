﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DAL;
using IvanGram.Configs;
using IvanGram.Models;
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

        public UserService(IMapper mapper, DataContext context, IOptions<AuthConfig> config, SessionService session)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
            _session = session;
        }

        public async Task<bool> CheckUserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var DBUser = _mapper.Map<DAL.Entities.User>(model);
            var temp = await _context.Users.AddAsync(DBUser);
            await _context.SaveChangesAsync();
            return temp.Entity.Id;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<DAL.Entities.User> GetUserByCredention (string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Email.ToLower() == login.ToLower());
            if (user == null)
                throw new Exception("User not found");

            if (!HashHelper.Verify(password, user.PasswordHash))
                throw new Exception("Password is incorrect");

            return user;
        }

        public async Task<DAL.Entities.User> GetUserById(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.Id == id);
            if (user == null)
                throw new Exception("User not found");
            return user;
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);
            return _mapper.Map<UserModel>(user);
        }

        private TokenModel GenerateTokens(DAL.Entities.UserSession userSession)
        {
            if (userSession.User == null)
                throw new Exception("You are wizard. This exception cant be throwed. Go and check your magic abilities!!! ^_^");

            var DTNow = DateTime.Now;

            var acessToken = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: DTNow,
                claims: new Claim[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userSession.User.Name),
                    new Claim("SessionId", userSession.Id.ToString()),
                    new Claim("Id", userSession.User.Id.ToString())
                },
                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );

            var encodedAcessToken = new JwtSecurityTokenHandler().WriteToken(acessToken);

            var refreshToken = new JwtSecurityToken(
                notBefore: DTNow,
                claims: new Claim[]
                {
                    new Claim("RefreshToken", userSession.RefreshToken.ToString())
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

            if (principal.Claims.FirstOrDefault(x => x.Type == "RefreshToken")?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshId))
            {
                var session = await _session.GetSessionByRefreshToken(refreshId);
                if (!session.IsActive)
                    throw new Exception("Session does not exist");
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

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
