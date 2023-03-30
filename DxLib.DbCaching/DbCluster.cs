using DXLib.ClusterList;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;

namespace DxLib.DbCaching
{
    public class DbCluster : DbCommon<ClusterRecord>
    {
        public DbCluster(IOptions<DbCacheOptions> options) : base(options)
        {
        }
        public async Task<ClusterRecord> GetMostRecentClusterAsync()
        {
            return await base.BaseGetIQueryable()
                .OrderByDescending(x => x.RetrievalDate)
                .FirstAsync();
        }
    }
}
