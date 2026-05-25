using LocalQuest.Models._2020;
using Microsoft.IdentityModel.Tokens;
using QuerryNetworking.Core;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LocalQuest.Controllers.ServiceControllers
{
    public class Auth : ClientRequest
    {
        private const string JwtSecret = "localquest-secret";

        [Get("/cachedlogin/forplatformid/{var}/{var}")]
        public List<CachedLogin> GetLogins(string Platform, string PlatformId)
        {
            return new List<CachedLogin>()
            {
                new CachedLogin()
            };
        }

        [Get("/eac/challenge")]
        public string EacChallenge()
        {
            return "\"" + Guid.NewGuid().ToString() + "\"";
        }

        [Post("/connect/token")]
        public TokenResponse ConnectToken()
        {
            string account_id = (Form["account_id"] ?? "").Trim('\'');
            string steamId = Config.GetString("SteamId") ?? "";
            var now = DateTimeOffset.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account_id),
                new Claim("rn.plat", "0"),
                new Claim("rn.platid", steamId),
                new Claim("client_id", "rec"),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            };

            foreach (var role in new[] { "screenshare", "gameClient" })
                claims.Add(new Claim("role", role));

            foreach (var scope in new[] { "openid", "rn.api", "rn.commerce", "rn.notify", "rn.match", "rn.chat", "rn.accounts", "rn.auth", "rn.link", "rn.lists", "rn.clubs", "rn.rooms", "rn.data", "offline_access" })
                claims.Add(new Claim("scope", scope));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5051",
                audience: "http://localhost:5051",
                claims: claims,
                notBefore: now.UtcDateTime,
                expires: now.AddHours(1).UtcDateTime,
                signingCredentials: creds
            );

            return new TokenResponse()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString(),
            };
        }
    }
}
