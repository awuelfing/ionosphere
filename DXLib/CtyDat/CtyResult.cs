using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.CtyDat
{
    public class CtyResult
    {
        //Added accessors to make this serialize in ASP.NET Core, not needed otherwise
        public string Callsign { get; set; } = string.Empty;
        public string DXCCEntityName { get; set; } = string.Empty;
        public string CQZone { get; set; } = string.Empty;
        public string ITUZone { get; set; } = string.Empty;
        public string Continent { get; set; } = string.Empty;
        public string Lat { get; set; } = string.Empty;
        public string Long { get; set; } = string.Empty;
        public string TZOffset { get; set; } = string.Empty;
        public string PrimaryPrefix { get; set; } = string.Empty;
        public bool ExactMatch { get; set; } = false;
    }
}
