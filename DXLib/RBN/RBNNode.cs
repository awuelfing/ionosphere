using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.RBN
{
    public class RBNNode
    {
        public string Station { get; set; } = string.Empty;
        public string MaidenheadLocator { get; set; } = string.Empty;
        public string PrimaryPrefix { get; set; } = string.Empty;
        public string Continent { get; set; } = string.Empty;
        public string CQZone { get; set; } = string.Empty;
        public string ITUZone { get; set; } = string.Empty;
    }
}
