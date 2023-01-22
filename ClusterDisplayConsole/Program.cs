using ClusterConnection;
using Microsoft.Extensions.Configuration;

namespace ClusterDisplayConsole
{
    internal class Program
    {
        static List<SpotEventArgs> _spots = new List<SpotEventArgs>();
        static string[] _callsignlist;
        static IConfigurationRoot _config;
        static Queue<SpotEventArgs> _recentspots = new Queue<SpotEventArgs>();
        static DateTime _lastScreenUpdate = DateTime.MinValue;

        static void PrintSpots(string callsign)
        {
            Console.Write($"{callsign}:");
            var myspots = _spots.Where(x => x.Spottee == callsign);
            if (myspots.Count() == 0)
            {
                Console.WriteLine("\t\t\tNo spots");
            }
            else
            {
                //todo: break this out by continent, add in band (plenty of vertical space)
                foreach (var myspot in myspots.OrderByDescending(x => x.Signal))
                {
                    Console.Write($"\t{myspot.Spotter}[{myspot.Signal}dB]({Math.Round((DateTime.Now-myspot.ReceivedDateTime).TotalMinutes)}m)");
                }
                Console.WriteLine();
            }
        }
        static void UpdateConsole()
        {
            
            Console.Clear();
            foreach(string callsign in _callsignlist)
            {
                PrintSpots(callsign);
            }
            Console.WriteLine("-------------------------");
            foreach (SpotEventArgs e in _recentspots.ToList())
            {
                Console.WriteLine(e.FormatSpot());
            }
            _lastScreenUpdate = DateTime.Now;
        }
        static void ReceiveSpot(object sender, SpotEventArgs e)
        {
            //Console.WriteLine($"{e.Spotter} saw {e.Spottee}");
            if(_callsignlist.Contains(e.Spottee))
            {
                _spots.Add(e);
            }
            _recentspots.Enqueue(e);
            if(_recentspots.Count() > _config.GetValue<int>("RetainRecentSpots"))
            {
                _recentspots.Dequeue();
            }
            if ((DateTime.Now - _lastScreenUpdate).TotalSeconds >5 ) UpdateConsole();
        }
        static void ReceiveDisconnect(object sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
        }
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            _config = configurationBuilder.Build();

            _callsignlist = _config.GetSection("Callsigns").Get<string[]>();

            
            ClusterClient clusterClient = new ClusterClient(_config["Server:Name"], _config.GetValue<int>("Server:Port"), _config["Server:PrimaryCallsign"]);
            if (!clusterClient.Connect())
            {
                Console.WriteLine("Failed to connect.");
                return;
            }
            clusterClient.SpotReceived += ReceiveSpot;
            clusterClient.Disconnected += ReceiveDisconnect;
            UpdateConsole();
            clusterClient.ProcessSpots();

        }
    }
}
