using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.ScpDb
{
    public class Scp
    {
        private readonly static string _ScpData;
        static Scp()
        {
            _ScpData = File.ReadAllText(AppContext.BaseDirectory + "MASTER.SCP");
        }
        public static bool MatchScp(string callsign)
        {
            if(_ScpData.Contains(callsign,StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
