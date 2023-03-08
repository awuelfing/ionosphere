using DxLib.DbCaching;
using DXLib.Cohort;
using DXLib.HamQTH;
using DXLib.RBN;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DXLib.WebAdapter
{
    public class WebAdapterClient : IQthLookup
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

        public async Task<HamQTHResult?> GetGeoAsync(string callsign, bool resolveDeeper = true)
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync($"/api/lookups/HamQTH/GetFullRecord?callsign={callsign}&LookLower={resolveDeeper}");
            //return await _httpClient.GetFromJsonAsync<HamQTHResult?>($"/api/lookups/HamQTH/GetFullRecord?callsign={callsign}&LookLower={resolveDeeper}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string result = await httpResponseMessage.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<HamQTHResult>(result);
            }
            return null;
        }
        public async Task DoBlindRetrieve(string uri)
        {
            await _httpClient.GetAsync(uri);
            return;
        }
        public async Task Enqueue(string callsign,int count)
        {
            await _httpClient.GetAsync($"api/lookups/HamQTH/Enqueue?callsign={callsign}&count={count}");
        }
        public async Task<string?> Dequeue()
        {
            var QthRecord = await _httpClient.GetFromJsonAsync<DbQueueRecord>("/api/lookups/HamQTH/Dequeue");
            return QthRecord?.Callsign;
        }
        public async Task PostSpotAsync(Spot spot)
        {
            await _httpClient.PostAsJsonAsync("/api/spots", spot);
        }
        public async Task<CohortRecord?> GetCohort(string Username)
        {
            CohortRecord? result;
            try
            {
                result = await _httpClient.GetFromJsonAsync<CohortRecord>($"/api/cohort/{Username}");
            }
            catch (Exception)
            {
                result = new CohortRecord() { Username = Username };
            }
            return result;
        }
        public async Task PostSummary(SummaryRecord summaryRecord)
        {
            await _httpClient.PostAsJsonAsync("/api/spot/summary", summaryRecord);
        }
    }
}
