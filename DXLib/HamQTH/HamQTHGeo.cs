using Microsoft.Extensions.Options;
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
    public class HamQTHGeo : IQthLookup
    {
        private readonly HamQTHOptions _options;
        private static string _sessionID = string.Empty;
        private static DateTime _sessionIssued = DateTime.MinValue;
        private static readonly HttpClient _httpClient;
        private static readonly string _loginRegex = @"\<session_id\>(\w+)\<\/session_id\>";
        private readonly NameValueCollection _workingQS;
        private readonly XmlSerializer _xmlSerializer;
        private static readonly SemaphoreSlim _semaphoreSlim = new(1);

        static HamQTHGeo()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.hamqth.com/")
            };
        }
        public HamQTHGeo(IOptions<HamQTHOptions> hamQTHOptions)
        {
            _xmlSerializer = new XmlSerializer(typeof(HamQTHResult));
            _workingQS = HttpUtility.ParseQueryString(string.Empty);
            _options = hamQTHOptions.Value;
        }
        private async Task DoLogin()
        {
            _workingQS.Clear();
            _workingQS["u"] = _options.Username;
            _workingQS["p"] = _options.Password;
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
        public async Task<HamQTHResult?> GetGeoAsync(string callsign, bool resolveDeeper = true)
        {
            string result;
            HamQTHResult? hamQTHResult;
            await CheckSession();

            _workingQS.Clear();
            _workingQS["id"] = _sessionID;
            _workingQS["callsign"] = callsign;
            _workingQS["prg"] = "IONOSPHERE";

            DateTime start = DateTime.Now;
            try
            {
                result = await _httpClient.GetStringAsync("xml.php?" + _workingQS.ToString());
            } catch (Exception ex)
            {
                throw new Exception("HttpClient failure", ex);
            }

            Debug.WriteLine($"Retrieved reponse from HamQTH in {(DateTime.Now - start).TotalMilliseconds}ms");

            if (string.IsNullOrEmpty(result)) throw new Exception("Communication failure");

            try
            {
                using StringReader stringReader = new(result);
                hamQTHResult = (HamQTHResult?)_xmlSerializer.Deserialize(stringReader);
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to deserialize: {result??"empty"}", ex);
            }

            if (hamQTHResult == null) return new HamQTHResult()
            {
                callsign = callsign.ToUpper(),
                firstretrieved = DateTime.Now,
                lastretrieved = DateTime.Now,
                status = "current"
            };

            hamQTHResult.callsign = callsign.ToUpper();
            hamQTHResult.firstretrieved = DateTime.Now;
            hamQTHResult.lastretrieved = DateTime.Now;
            hamQTHResult.status = "current";

            //hamQTHResult!.SearchResult!.nick = hamQTHResult!.SearchResult!.nick!.ToUpper();

            return hamQTHResult;
        }

    }
}
