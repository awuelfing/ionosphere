using Amazon.SecurityToken.Model;
using DxLib.DbCaching;
using DXLib.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DXws.Admin
{
    public static class AdminHelp
    {
        public static string GetUser(IEnumerable<Claim> claims)
        {
            Claim? c = claims?.Where(x => x.Type == ClaimTypes.Name)?.FirstOrDefault();
            return (c?.Value)??string.Empty;
        }
        public static AuthenticationProperties GetAuthenticationProperties(int days = 180)
        {
            return new AuthenticationProperties()
            {
                IsPersistent = true,
                AllowRefresh = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(days)
            };
        }
        public static ClaimsPrincipal CreateClaimsUserRoles(string username, IEnumerable<string> roles)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, username));
            foreach(string role in roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            return new ClaimsPrincipal(claimsIdentity);
        }
        public static string GenerateJWT(UserRecord userRecord,string key)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, userRecord.Username));
            foreach(string role in userRecord.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                Issuer = "me",
                Audience = "me",
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
