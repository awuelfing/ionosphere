using Amazon.SecurityToken.Model;
using System.Security.Claims;

namespace DXws.Admin
{
    public class AdminHelp
    {
        public static string GetUser(IEnumerable<Claim> claims)
        {
            Claim? c = claims?.Where(x => x.Type == ClaimTypes.Name)?.FirstOrDefault();
            return (c?.Value)??string.Empty;
        }
    }
}
