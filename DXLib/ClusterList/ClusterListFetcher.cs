using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DXLib.ClusterList
{
    public class ClusterListFetcher
    {
        private static HttpClient _httpClient;
        public const string ClusterUri = "https://www.n1mm-lib.hamdocs.com/_getfromclusterratings.php";
        private const string ClusterRegex = @"(?<Continent>\w{2})\s-\s(?<DisplayName>[\w\.:\d]+)\s\((?<Users>\d+)\s\/\s(?<Success>\d+)%\),\s(?<Host>[\w\.]+):(?<Port>\d+)";

        static ClusterListFetcher()
        {
            _httpClient = new HttpClient();
        }

        public static async Task<List<ClusterNodeRaw>> FetchClusterResultsAsync()
        {
            List<ClusterNodeRaw> nodes = new List<ClusterNodeRaw>();
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(ClusterUri);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string content = await httpResponseMessage.Content.ReadAsStringAsync();
                MatchCollection mc = Regex.Matches(content, ClusterRegex, RegexOptions.Singleline);
                foreach (Match match in mc)
                {
                    nodes.Add(new ClusterNodeRaw()
                    {
                        ReportingContinent = match.Groups["Continent"].Value,
                        DisplayName = match.Groups["DisplayName"].Value,
                        Users = int.Parse(match.Groups["Users"].Value),
                        Success = int.Parse(match.Groups["Success"].Value),
                        Host = match.Groups["Host"].Value,
                        Port = int.Parse(match.Groups["Port"].Value)
                    });
                }
                return nodes;
            }
            else throw new Exception("Failed to retrieve cluster list");
        }

    }
}
