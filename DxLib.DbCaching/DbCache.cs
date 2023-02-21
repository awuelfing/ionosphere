using DXLib.HamQTH;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DxLib.DbCaching
{
    public class DbCache : DbCommon<HamQTHResult>, IQthLookup
    {
        public IQthLookup? _qthLookup = null;

        public DbCache(IOptions<DbCacheOptions> options) : base(options)
        {
        }

        public DbCache(DbCacheOptions options) : base(options)
        {
        }

        public async Task<HamQTHResult?> GetGeoAsync(string callsign, bool resolveDeeper = true)
        {
            var filter = Builders<HamQTHResult>.Filter.Eq("callsign", callsign.ToUpper());
            var result = await base.BaseGetOneAsync(filter);

            if (result != null)
            {
                return result;
            }

            if (_qthLookup == null || !resolveDeeper)
            {
                return null;
            }
            result = await _qthLookup.GetGeoAsync(callsign);

            if(result != null)
            {
                await base.BaseStoreOneAsync(result);
                return result;
            }

            return result;
        }

    }
}