using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IvanGram.Configs
{
    public class AuthConfig
    {
        public const string Position = "Auth";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public int LifeTime { get; set; }
        public SymmetricSecurityKey SymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(Key));
    }
}
