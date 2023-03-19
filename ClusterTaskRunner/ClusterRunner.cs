using ClusterConnection;
using DXLib.CtyDat;
using DXLib.RBN;
using DXLib.WebAdapter;
using Microsoft.Extensions.Hosting;
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
        private readonly IHostApplicationLifetime _appLife;
        public int ConnectionRetries { get; set; } = 0;

        public ClusterRunner(ILogger<ClusterRunner> logger, IOptions<ClusterClientOptions> clusterClientOptions, IHostApplicationLifetime appLifetime)
        {
            _logger= logger;
            _clusterClientOptions = clusterClientOptions.Value;
            _appLife = appLifetime;
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

            bool connectionAttemptStatus = false;
            try
            {
                connectionAttemptStatus = await _clusterClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection exception");
                connectionAttemptStatus = false;
            }

            if (connectionAttemptStatus == false)
            {
                _logger.LogError("Cluster connection to {0}:{1} could not connect, requesting program exit", 
                    _clusterClientOptions.Host, 
                    _clusterClientOptions.Port);
                _appLife.StopApplication();
            }
            else
            {
                _logger.Log(LogLevel.Information,
                "Cluster connection to {0}:{1} succeeded",
                _clusterClientOptions.Host,
                _clusterClientOptions.Port);
            }
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {

        }
        public void ReceiveDisconnect(object? sender, EventArgs e)
        {
            _logger.LogError("Cluster connection to {0}:{1} disconnected", 
                _clusterClientOptions.Host, 
                _clusterClientOptions.Port);
            ConnectionRetries++;
            if (this.ConnectionRetries < _clusterClientOptions.ConnectionAttempts)
            {
                Thread.Sleep(5000);
                try
                {
                    _clusterClient!.Connect();
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "failed to reconnect, requesting program exit");
                    _appLife.StopApplication();
                }
            }
            else
            {
                _logger.Log(LogLevel.Error, "Connection attempts ({0}) exceeded, requesting program exit", _clusterClientOptions.ConnectionAttempts);
                _appLife.StopApplication();
            }
        }
    }
}
