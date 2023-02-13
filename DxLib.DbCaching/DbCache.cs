using DXLib.HamQTH;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace DxLib.DbCaching
{
    public class DbCache : DbCommon<HamQTHResult>, QthLookup
    {
        public QthLookup? _qthLookup;

        static DbCache()
        {
            BsonClassMap.RegisterClassMap<HamQTHResult>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        public DbCache(IOptions<DbCacheOptions> options) : base(options)
        {
        }

        public DbCache(DbCacheOptions options) : base(options)
        {
        }


        public async Task StoreResult(HamQTHResult result)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }
            await _mongoCollection!.InsertOneAsync(result);

            return;
        }

        public async Task<HamQTHResult?> GetGeoAsync(string callsign)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }

            DateTime start = DateTime.UtcNow;
            var filter = Builders<HamQTHResult>.Filter.Eq("callsign", callsign.ToUpper());
            var results = await _mongoCollection.FindAsync(filter);
            var result = await results.FirstOrDefaultAsync();
            Debug.WriteLine($"DB cache checked in {(start - DateTime.Now).TotalMilliseconds}ms");

            if (result != null)
            {
                Debug.WriteLine("DB cache hit");
                return result;
            }

            if (_qthLookup == null)
            {
                return null;
            }
            result = await _qthLookup.GetGeoAsync(callsign);

            if(result != null)
            {
                start= DateTime.Now;
                await StoreResult(result);
                Debug.WriteLine($"Stored result from lower provider in {(DateTime.Now - start).TotalMilliseconds}ms");
                return result;
            }

            return result;
        }

    }
}