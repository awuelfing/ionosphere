using DXLib.ClusterList;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbCluster : DbCommon<ClusterRecord>
    {
        public DbCluster(IOptions<DbCacheOptions> options) : base(options)
        {
        }
    }
}
