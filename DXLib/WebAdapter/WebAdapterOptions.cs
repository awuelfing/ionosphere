using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.WebAdapter
{
    public class WebAdapterOptions
    {
        public const string WebAdapter = "WebAdapter";
        public string BaseURL { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;
    }
}
