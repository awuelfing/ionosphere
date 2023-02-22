using ClusterConnection;
using DXLib.CtyDat;
using DXLib.RBN;
using DXLib.WebAdapter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterTaskRunner
{
    public class ClusterRunner
    {
        public ClusterClient? _clusterClient;
        private readonly ClusterClientOptions _clusterClientOptions;

        public ClusterRunner(IOptions<ClusterClientOptions> clusterClientOptions)
        {
            _clusterClientOptions = clusterClientOptions.Value;
            //this is light enought to put in the constructor
            _clusterClient = new ClusterClient(_clusterClientOptions.Host, _clusterClientOptions.Port, _clusterClientOptions.Callsign);
        }
        public void Initialize()
        {
            _clusterClient!.SpotReceived += this.ReceiveSpots;
            _clusterClient!.Disconnected += this.ReceiveDisconnect;
            if (_clusterClient!.Connect())
            {
                Console.WriteLine("Failed to connect.");
                return;
            }
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {

        }
        public void ReceiveDisconnect(object? sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
        }
    }
}
