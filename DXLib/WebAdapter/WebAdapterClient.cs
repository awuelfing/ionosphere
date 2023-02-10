using DXLib.HamQTH;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.WebAdapter
{
    public class WebAdapterClient : QthLookup
    {
        private static HttpClient _httpClient;
        
        static WebAdapterClient()
        {
            _httpClient = new HttpClient();
        }
        public WebAdapterClient(IOptions<WebAdapterOptions> options)
        {
            _httpClient.BaseAddress = new Uri(options.Value.BaseURL);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.Value.JwtToken}");
        }
        public WebAdapterClient(WebAdapterOptions options)
        {
            _httpClient.BaseAddress = new Uri(options.BaseURL);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.JwtToken}");
        }

        public override async Task<HamQTHResult?> GetGeoAsync(string callsign)
        {
            return await _httpClient.GetFromJsonAsync<HamQTHResult?>($"/api/lookups/HamQTH/GetFullRecord?callsign={callsign}");
        }
    }
}
