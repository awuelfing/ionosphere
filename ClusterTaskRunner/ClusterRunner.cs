using ClusterConnection;
using DXLib.CtyDat;
using DXLib.RBN;
using DXLib.WebAdapter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
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
        private readonly ILogger<ClusterRunner> _logger;

        public ClusterRunner(ILogger<ClusterRunner> logger, IOptions<ClusterClientOptions> clusterClientOptions)
        {
            _logger= logger;
            _clusterClientOptions = clusterClientOptions.Value;
            //this is light enought to put in the constructor
            _clusterClient = new ClusterClient(_clusterClientOptions.Host, _clusterClientOptions.Port, _clusterClientOptions.Callsign);
        }
        public async Task InitializeAsync()
        {
            _clusterClient!.SpotReceived += this.ReceiveSpots;
            _clusterClient!.Disconnected += this.ReceiveDisconnect;
            _logger.Log(LogLevel.Information,"Cluster connection to {0}:{1} attempting to connect",
                _clusterClientOptions.Host,
                _clusterClientOptions.Port);
            if (! await _clusterClient!.ConnectAsync())
            {
                _logger.LogError("Cluster connection to {0}:{1} could not connect", 
                    _clusterClientOptions.Host, 
                    _clusterClientOptions.Port);
                return;
            }
            _logger.Log(LogLevel.Information,
                "Cluster connection to {0}:{1} succeeded",
                _clusterClientOptions.Host,
                _clusterClientOptions.Port);
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {

        }
        public void ReceiveDisconnect(object? sender, EventArgs e)
        {
            _logger.LogError("Cluster connection to {0}:{1} disconnected", 
                _clusterClientOptions.Host, 
                _clusterClientOptions.Port);
        }
    }
}
