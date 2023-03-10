using DXLib.RBN;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxLib.DbCaching
{
    public class DbSpots : DbCommon<Spot>
    {
        public DbSpots(IOptions<DbCacheOptions> options) : base(options)
        {
        }

        public DbSpots(DbCacheOptions options) : base(options)
        {
        }
        public async Task<List<Spot>> GetAllSpotsAsync(string Callsign)
        {
            var filter = Builders<Spot>.Filter.Eq("Spottee", Callsign.ToUpper());
            return await base.BaseGetManyAsync(filter);
        }
        public async Task<List<Spot>> GetAllCohortSpotsAsync(string[] calls)
        {
            var filter = Builders<Spot>.Filter.In("Spottee", calls);
            return await base.BaseGetManyAsync(filter);
        }
        public async Task<List<Spot>> GetAllCohortSpotsAsync(string[] calls,int Lookback)
        {
            DateTime dateTime = DateTime.UtcNow.AddMinutes(-Lookback);
            var filter = Builders<Spot>.Filter.In("Spottee", calls) &
                Builders<Spot>.Filter.Gt("ReceivedDateTime", dateTime);
            return await base.BaseGetManyAsync(filter);
        }
    }
}
