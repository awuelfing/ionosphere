using Microsoft.Extensions.Configuration;
using ClusterConnection;
using System.Collections.Concurrent;
using DXLib.WebAdapter;
using DXLib.HamQTH;
using System.Diagnostics;
using DXLib.Cohort;
using DXLib.RBN;
using DXLib.CtyDat;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System.Runtime.InteropServices;

namespace ClusterTaskRunner
{
    internal class Program
    {
        private const string _purgeUri = "/api/spots/DeleteOld";
        static async Task Main(string[] args)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationRoot)
                .Enrich.WithThreadId()
                .CreateLogger();

            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<WebAdapterClient>();
                services.AddSingleton<WebTaskRunner>();
                services.AddSingleton<QueueRunner>();
                services.AddSingleton<SpotReporter>();
                services.AddSingleton<ClusterRunner>();
                services.AddSingleton<StatusReporter>();
                services.Configure<ProgramOptions>(configurationRoot.GetSection(ProgramOptions.ProgramOptionName));
                services.Configure<ClusterClientOptions>(configurationRoot.GetSection(ClusterClientOptions.ClusterClient));
                services.Configure<WebAdapterOptions>(configurationRoot.GetSection(WebAdapterOptions.WebAdapter));
                services.AddLogging(builder => builder.AddSerilog(Log.Logger));
            });

            var host = builder.Build();
            
            var options = host.Services.GetRequiredService<IOptions<ProgramOptions>>().Value;
            var clusterConnection = host.Services.GetRequiredService<ClusterRunner>();

            /*
            WebAdapterOptions webAdapterOptions = new WebAdapterOptions();
            configurationRoot.GetSection(WebAdapterOptions.WebAdapter).Bind(webAdapterOptions);
             _webAdapterClient = new WebAdapterClient(webAdapterOptions);
            */

            Log.Debug("Services registered");

            if(options.EnableStatusReport)
            {
                var statusReport = host.Services.GetRequiredService<StatusReporter>();
                _ = Task.Factory.StartNew(statusReport.StatusLoop, TaskCreationOptions.LongRunning);
            }

            if (options.EnableQueueUploader)
            {
                var queueUploader = host.Services.GetRequiredService<QueueRunner>();
                clusterConnection._clusterClient!.SpotReceived += queueUploader.ReceiveSpots;
                _ = Task.Factory.StartNew(queueUploader.PumpQueue, TaskCreationOptions.LongRunning);
                Log.Debug("Queue uploader startup complete");
            }

            if(options.EnableQueueResolver)
            {
                _ = Task.Factory.StartNew(
                    host.Services.GetRequiredService<QueueRunner>().ProcessResolver,
                    TaskCreationOptions.LongRunning);
                Log.Debug("Queue resolver startup complete");
            }
            if(options.EnableSpotUpload)
            {
                var spotUploader = host.Services.GetRequiredService<SpotReporter>();
                await spotUploader.PopulateCohorts();

                clusterConnection._clusterClient!.SpotReceived += spotUploader.ReceiveSpots;
                if (options.EnableStatusReport)
                {
                    var statusReport = host.Services.GetRequiredService<StatusReporter>();
                    spotUploader.SpotUploaded += statusReport.ReportSpotUpload;
                }
                _ = Task.Factory.StartNew(
                        spotUploader.PumpSpots,
                        TaskCreationOptions.LongRunning);
                Log.Debug("Spot upload startup complete");
            }
            if(options.EnableClusterConnection)
            {
                clusterConnection.Initialize();
                _ = Task.Factory.StartNew(() => clusterConnection._clusterClient!.ProcessSpots(),
                    TaskCreationOptions.LongRunning);
                Log.Debug("Cluster connection initalized and startup complete");
            }
            if(options.EnableKeepAlive)
            {
                var keepaliveDelay = options.KeepAliveDelay;
                var keepalive = () =>
                {
                    _ = host.Services.GetRequiredService<WebTaskRunner>()
                        .RunWebTask(string.Empty, keepaliveDelay)
                        .ConfigureAwait(false);
                };
                _ = Task.Factory.StartNew(keepalive, TaskCreationOptions.LongRunning);
                Log.Debug("Keepalive startup complete");
            }
            if (options.EnableSpotPurge)
            {
                var spotPurgeDelay = options.SpotPurgeDelay;
                var rightUri = $"{_purgeUri}?minutes={options.SpotPurgeAgeMinutes}";
                var spotPurge = () =>
                {
                    _ = host.Services.GetRequiredService<WebTaskRunner>()
                        .RunWebTask(rightUri, spotPurgeDelay)
                        .ConfigureAwait(false);
                };
                _ = Task.Factory.StartNew(spotPurge, TaskCreationOptions.LongRunning);
                Log.Debug("Spot Purge startup complete");
            }
            await host.RunAsync();
        }
    }
}