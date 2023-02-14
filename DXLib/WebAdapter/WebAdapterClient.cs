﻿using DxLib.DbCaching;
using DXLib.HamQTH;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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

        public async Task<HamQTHResult?> GetGeoAsync(string callsign)
        {
            return await _httpClient.GetFromJsonAsync<HamQTHResult?>($"/api/lookups/HamQTH/GetFullRecord?callsign={callsign}");
        }
        public async Task DoKeepAlive()
        {
            await _httpClient.GetAsync("/api/lookups/scp?callsign=W1AW");
            return;
        }
        //
        public async Task Enqueue(string callsign,int count)
        {
            await _httpClient.GetAsync($"api/lookups/HamQTH/Enqueue?callsign={callsign}&count={count}");
        }
        public async Task<string?> Dequeue()
        {
            var QthRecord = await _httpClient.GetFromJsonAsync<DbQueueRecord>("/api/lookups/HamQTH/Dequeue");
            return QthRecord?.Callsign;
        }

    }
}
