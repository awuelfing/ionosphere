using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.User
{
    public class UserRecord
    {
        public string? Username { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DefaultLookback { get; set; } = 5;
    }
}
