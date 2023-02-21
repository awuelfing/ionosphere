using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ClusterConnection
{
    public class ClusterClient
    {
        private TcpClient _client;
        private Byte[] _outbuffer;
        public bool Connected = false;
        public int Spots = 0;
        public event EventHandler<SpotEventArgs> SpotReceived;
        public event EventHandler Disconnected;
        //private static readonly string _spotRegex = @"^DX de (?<Spotter>.{11})(?<Frequency>.{9})(?<Spottee>.{13})(?<Comment>.+)(?<Time>.{4})Z$";
        private static readonly string _spotRegexNew = @"^DX\sde\s(?<Spotter>[\w-#\/]+):\s*(?<Frequency>[\d\.]+)\s+(?<Spottee>[\w\d\/]+)(?<Comment>.*)(?<Time>\d{4}Z)(\s\w{2}\d{2}){0,1}$";
        private string _server, _username;
        private int _port = 0;
        private NetworkStream _ns = null;
        private StreamReader _sr = null;

        public ClusterClient(string server, int port, string username)
        {
            _server = server;
            _username = username;
            _port = port;
        }

        public bool Connect()
        {
            _client = new TcpClient(_server, _port);
            if (!_client.Connected) return false;
            Thread.Sleep(500);

            _outbuffer = System.Text.Encoding.ASCII.GetBytes($"{_username}\n");
            _ns = _client.GetStream();
            _sr = new StreamReader(_ns);
            _ns.Write(_outbuffer, 0, _outbuffer.Length);

            Connected = true;
            return true;
        }
        public void ProcessSpots()
        {

            try
            {
                while (_client.Connected)
                {
                    var line = _sr.ReadLine();
                    //Console.WriteLine(line);
                    Debug.WriteLine(line);
                     MatchCollection spotCollection = Regex.Matches(line ?? "", _spotRegexNew, RegexOptions.Multiline);

                    foreach (Match spotMatch in spotCollection)
                    {
                        Spots++;
                        EventHandler<SpotEventArgs> eventHandler = this.SpotReceived;
                        if (eventHandler != null)
                        {
                            eventHandler(this, new SpotEventArgs(spotMatch.Groups["Spotter"].ToString().Trim().Replace(":","").Replace("-#",""),
                                spotMatch.Groups["Frequency"].ToString().Trim(),
                                spotMatch.Groups["Spottee"].ToString().Trim(),
                                spotMatch.Groups["Comment"].ToString().Trim(),
                                spotMatch.Groups["Time"].ToString().Trim()
                                ));
                        }
                    }
                }
            }
            finally
            {
                Connected = false;
                if(_sr != null) _sr.Dispose();
                if(_ns != null) _ns.Dispose();
                if (_client != null) _client.Dispose();
                EventHandler eventHandler = this.Disconnected;
                if(eventHandler != null)
                {
                    eventHandler(this, new EventArgs());
                }
            }


        }
    }
}