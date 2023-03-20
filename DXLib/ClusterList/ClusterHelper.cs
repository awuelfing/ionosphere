using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DXLib.ClusterList
{
    public static class ClusterHelper
    {
        public static List<ClusterNode> AsClusterNodes(this List<ClusterNodeRaw> nodes)
        {
            var result = new List<ClusterNode>();
            foreach (var node in nodes)
            {
                var clusterNode = result.Where(x => x.DisplayName == node.DisplayName).FirstOrDefault();
                if (clusterNode != null)
                {
                    clusterNode.ContinentDetails!.Add(node.AsContinentDetail());
                }
                else
                {
                    clusterNode = node.AsClusterNode();
                    clusterNode.ContinentDetails = new List<ClusterNodeContinentDetail>()
                        {
                            node.AsContinentDetail()
                        };
                    result.Add(clusterNode);
                }
            }
            return result;
        }
    }
}
