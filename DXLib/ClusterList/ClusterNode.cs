using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.ClusterList
{
    public class ClusterNode
    {
        public string DisplayName { get; set; } = string.Empty;
        public int TotalUsers
        {
            get
            {
                return this.ContinentDetails?.Sum(x => x.Users) ?? 0;
            }
        }
        public int TotalSuccess
        {
            get
            {
                return (int) Math.Round( this.ContinentDetails?.Average(x => x.Success) ?? 0);
            }
        }
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public List<ClusterNodeContinentDetail>? ContinentDetails { get; set; }
    }
}
