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
                services.AddSingleton<KeepaliveRunner>();
                services.AddSingleton<QueueRunner>();
                services.AddSingleton<SpotReporter>();
                services.AddSingleton<ClusterRunner>();
                services.Configure<ProgramOptions>(configurationRoot.GetSection(ProgramOptions.ProgramOptionName));
                services.Configure<ClusterClientOptions>(configurationRoot.GetSection(ClusterClientOptions.ClusterClient));
                services.Configure<WebAdapterOptions>(configurationRoot.GetSection(WebAdapterOptions.WebAdapter));
            });


            var host = builder.Build();

            var options = host.Services.GetRequiredService<IOptions<ProgramOptions>>().Value;
            var clusterConnection = host.Services.GetRequiredService<ClusterRunner>();

            /*
            WebAdapterOptions webAdapterOptions = new WebAdapterOptions();
            configurationRoot.GetSection(WebAdapterOptions.WebAdapter).Bind(webAdapterOptions);
             _webAdapterClient = new WebAdapterClient(webAdapterOptions);
            */

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            if (options.EnableQueueUploader)
            {
                var queueUploader = host.Services.GetRequiredService<QueueRunner>();
                clusterConnection._clusterClient!.SpotReceived += queueUploader.ReceiveSpots;
                queueUploader.PumpQueue();
            }

            if(options.EnableQueueResolver)
            {
                host.Services.GetRequiredService<QueueRunner>().ProcessResolver();
            }
            if(options.EnableSpotUpload)
            {
                var spotUploader = host.Services.GetRequiredService<SpotReporter>();
                await spotUploader.PopulateCohorts();
                //todo - move this to a separate thread
                clusterConnection._clusterClient!.SpotReceived += spotUploader.ReceiveSpots;
            }
            if(options.EnableClusterConnection)
            {
                clusterConnection.Initialize();
                Task.Run(() => clusterConnection._clusterClient!.ProcessSpots());
            }
            if(options.EnableKeepAlive)
            {
                host.Services.GetRequiredService<KeepaliveRunner>().ProcessKeepAlive();
            }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            await host.RunAsync();
        }
    }
}