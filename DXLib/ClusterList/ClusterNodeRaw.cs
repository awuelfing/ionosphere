using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.ClusterList
{
    public class ClusterNodeRaw
    {
        public string ReportingContinent { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public int Users { get; set; } = 0;
        public int Success { get; set; } = 0;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public ClusterNodeContinentDetail AsContinentDetail()
        {
            return new ClusterNodeContinentDetail()
            {
                ReportingContinent = this.ReportingContinent,
                Users = this.Users,
                Success = this.Success
            };
        }
        public ClusterNode AsClusterNode()
        {
            return new ClusterNode()
            {
                DisplayName = this.DisplayName,
                Host = this.Host,
                Port = this.Port
            };
        }
    }
}
