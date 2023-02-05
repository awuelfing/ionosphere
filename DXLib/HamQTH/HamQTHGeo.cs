using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace DXLib.HamQTH
{
    public class HamQTHGeo : QthLookup
    {
        private static readonly string _username;
        private static readonly string _password;
        private static string _sessionID = string.Empty;
        private static DateTime _sessionIssued = DateTime.MinValue;
        private static readonly HttpClient _httpClient;
        private static readonly string _loginRegex = @"\<session_id\>(\w+)\<\/session_id\>";
        private readonly NameValueCollection _workingQS;
        private readonly XmlSerializer _xmlSerializer;
        private static readonly SemaphoreSlim _semaphoreSlim = new(1);
        private static readonly IConfigurationRoot _config;

        static HamQTHGeo()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.hamqth.com/")
            };
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            _config = configurationBuilder.Build();
            _username = _config.GetValue<string>("HamQTH:Username","")??"";
            _password = _config.GetValue<string>("HamQTH:Password", "")??"";

        }
        public HamQTHGeo()
        {
            _xmlSerializer = new XmlSerializer(typeof(HamQTHResult));
            _workingQS = HttpUtility.ParseQueryString(string.Empty);
        }
        private async Task DoLogin()
        {
            _workingQS.Clear();
            _workingQS["u"] = _username;
            _workingQS["p"] = _password;
            string result = await _httpClient.GetStringAsync("/xml.php?" + _workingQS.ToString());

            Match match = Regex.Match(result ?? "", _loginRegex);
            if (match.Success)
            {
                _sessionID = match.Groups[1].Value;
                Debug.WriteLine($"Login success, session id is {_sessionID}");
                _sessionIssued = DateTime.Now;
            }
            else
            {
                throw new Exception("Login failed.");
            }
        }

        private static bool CheckSessionExpiration()
        {
            if ((DateTime.Now - _sessionIssued).TotalMinutes > 59)
            {
                return true;
            }
            return false;
        }

        public async Task CheckSession()
        {
            if(CheckSessionExpiration())
            {
                Debug.WriteLine("Session is old, locking");
                await _semaphoreSlim.WaitAsync();
                if(CheckSessionExpiration())
                {
                    Debug.WriteLine("Locked session, logging in");
                    await DoLogin();
                }
                _semaphoreSlim.Release();
            }
        }
        public override async Task<HamQTHResult?> GetGeo(string callsign)
        {
            await CheckSession();

            _workingQS.Clear();
            _workingQS["id"] = _sessionID;
            _workingQS["callsign"] = callsign;
            _workingQS["prg"] = "IONOSPHERE";

            DateTime start = DateTime.Now;
            string result = await _httpClient.GetStringAsync("xml.php?" + _workingQS.ToString());
            Debug.WriteLine($"Retrieved reponse from HamQTH in {(DateTime.Now-start).TotalMilliseconds}ms");

            if (string.IsNullOrEmpty(result)) throw new Exception("Communication failure");
            using StringReader stringReader = new(result);
            HamQTHResult? hamQTHResult = (HamQTHResult?)_xmlSerializer.Deserialize(stringReader);
            if (hamQTHResult == null)
            {
                Debug.WriteLine("No result found from HamQTH.");
                return null;
            }
            hamQTHResult.retrievaldate = DateTime.Now;
            hamQTHResult.status = "current";
            return hamQTHResult;
        }
    }
}
