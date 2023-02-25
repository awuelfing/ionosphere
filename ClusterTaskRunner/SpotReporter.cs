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
    public class SpotReporter
    {
        private readonly WebAdapterClient _webAdapterClient;
        private readonly ProgramOptions _programOptions;
        private readonly List<string> _cohorts = new List<string>();
        private readonly ILogger<SpotReporter> _logger;
        public event EventHandler? SpotUploaded;

        public SpotReporter(ILogger<SpotReporter> logger,WebAdapterClient webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _logger = logger;
            _webAdapterClient = webAdapterClient;
            _programOptions = programOptions.Value;
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            _logger.Log(LogLevel.Trace, "received {e}", e);
            if (_programOptions!.EnableSpotUpload && _cohorts.Any(x => x == e.Spottee))
            {
                _logger.Log(LogLevel.Trace, "qualified {e}", e);
                Spot spot = e.AsSpot();
                spot.SpotterStationInfo = RbnLookup.GetRBNNodeSync(spot.Spotter);
                if (spot.SpotterStationInfo == null)
                {
                    _logger.Log(LogLevel.Debug, "RBN lookup failed for {Spotter}", e.Spotter);
                    CtyResult? ctyResult = Cty.MatchCall(spot.Spotter);
                    if (ctyResult != null)
                    {
                        spot.SpotterStationInfo = new RBNNode()
                        {
                            Continent = ctyResult.Continent,
                            PrimaryPrefix = ctyResult.PrimaryPrefix,
                            CQZone = ctyResult.CQZone,
                            ITUZone = ctyResult.ITUZone,
                            Station = ctyResult.Callsign
                        };
                    }
                }
                spot.SpottedStationInfo = Cty.MatchCall(spot.Spottee);

                _ = _webAdapterClient!.PostSpotAsync(spot);
                _logger.Log(LogLevel.Trace, "deferred upload of {e}", e);

                EventHandler? eventHandler = this.SpotUploaded;
                if(eventHandler != null)
                {
                    eventHandler(this, new EventArgs());
                }
            }
        }
        public async Task PopulateCohorts()
        {
            if (_programOptions.EnableSpotUpload)
            {
                var users = _programOptions.Users.ToList();
                foreach (string s in users)
                {
                    _logger.Log(LogLevel.Information, "populating cohorts for {s}", s);
                    var cohorts = await _webAdapterClient.GetCohort(s);
                    foreach (string cohort in cohorts!.Cohorts)
                    {
                        _cohorts.Add(cohort);
                    }
                    _logger.Log(LogLevel.Information, "size of cohort list is {Count}", _cohorts.Count());
                }
            }
        }
    }
}
