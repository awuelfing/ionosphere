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
    public class SpotReporter
    {
        private readonly WebAdapterClient _webAdapterClient;
        private readonly ProgramOptions _programOptions;
        private static List<string> _cohorts = new List<string>();

        public SpotReporter(WebAdapterClient webAdapterClient, IOptions<ProgramOptions> programOptions)
        {
            _webAdapterClient = webAdapterClient;
            _programOptions = programOptions.Value;
        }

        public void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            Console.WriteLine("Spot reporter received spot");
            if (_programOptions!.EnableSpotUpload && _cohorts.Any(x => x == e.Spottee))
            {
                Spot spot = e.AsSpot();
                spot.SpotterStationInfo = RbnLookup.GetRBNNodeSync(spot.Spotter);
                if (spot.SpotterStationInfo == null)
                {
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _webAdapterClient!.PostSpotAsync(spot);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }
        public async Task PopulateCohorts()
        {
            if (_programOptions.EnableSpotUpload)
            {
                var users = _programOptions.Users.ToList();
                foreach (string s in users)
                {
                    var cohorts = await _webAdapterClient.GetCohort(s);
                    foreach (string cohort in cohorts!.Cohorts)
                    {
                        _cohorts.Add(cohort);
                    }
                }
            }
        }
    }
}
