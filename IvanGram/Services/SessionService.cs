using DAL;
using Microsoft.EntityFrameworkCore;

namespace IvanGram.Services
{
    public class SessionService : IDisposable
    {
        private readonly DataContext _context;

        public SessionService(DataContext context)
        {
            _context = context;
        }

        public async Task<DAL.Entities.UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);
            if (session == null)
                throw new Exception("Session does not exists");
            return session;
        }

        public async Task<DAL.Entities.UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
            if (session == null)
                throw new Exception("Session does not exists");
            return session;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
