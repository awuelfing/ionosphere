using DXLib.RBN;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
        public async Task<List<Spot>> GetAllSpotsForContinent(string callsign,string continent)
        {
            return await base.BaseGetIQueryable()
                .Where(x => x.Spottee == callsign)
                .Where(x => x.SpotterStationInfo != null)
                .Where(x => x.SpotterStationInfo!.Continent == continent)
                .ToListAsync();


            //&& x.SpotterStationInfo.Continent == continent).ToListAsync();
            // return await base._mongoCollection.AsQueryable()
            //   .Where(x => x.Spottee == callsign && x.SpotterStationInfo.Continent == continent).ToListAsync();
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
