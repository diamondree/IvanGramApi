using Common.Consts;
using Common.Extensions;
using DAL.Entities;
using IvanGram.Services;

namespace IvanGram.Middleware
{
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public TokenValidatorMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context, SessionService sessionService)
        {
            var IsOk = true;
            var SessionId = context.User.GetClaimValue<Guid>(ClaimNames.SessionId);
            if (SessionId != default)
            {
                var session = await sessionService.GetSessionById(SessionId);
                if (!session.IsActive)
                {
                    IsOk = false;
                    context.Response.Clear();
                    context.Response.StatusCode = 401;
                }
            }
            if (IsOk)
                await _requestDelegate(context);
        }
    }

    public static class TokenValidatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidator(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidatorMiddleware>();
        }
    }
}
