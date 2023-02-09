using DXLib.HamQTH;
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
    public class DbQueue
    {
        private readonly DbCacheOptions _options;
        private MongoClientSettings? _settings;
        private MongoClient? _mongoClient;
        private IMongoDatabase? _mongoDatabase;
        private IMongoCollection<DbQueueRecord>? _mongoCollection;
        private bool _initialized = false;
        public DbQueue(IOptions<DbCacheOptions> options)
        {
            _options = options.Value;
        }
        public DbQueue(DbCacheOptions options)
        {
            _options = options;
        }
        static DbQueue()
        {
            BsonClassMap.RegisterClassMap<DbQueueRecord>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
        public void Initialize() // moved this stuff out of the constructor
        {
            _settings = MongoClientSettings.FromConnectionString(_options.ConnectionString);
            _settings.SslSettings = new SslSettings()
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };
            _mongoClient = new MongoClient(_settings);

            _mongoDatabase = _mongoClient.GetDatabase(_options.Database);
            _mongoCollection = _mongoDatabase.GetCollection<DbQueueRecord>(_options.QueueCollection);
            _initialized = true;
        }
        public async Task EnqueueAsync(string callsign)
        {
            if (!this._initialized)
            {
                this.Initialize();
            }

            var filter = Builders<DbQueueRecord>.Filter.Eq("Callsign", callsign.ToUpper());
            var results = await _mongoCollection.FindAsync(filter);
            var result = await results.FirstOrDefaultAsync();
            if (result == null)
            {
                DbQueueRecord record = new DbQueueRecord() { Callsign = callsign };
                await _mongoCollection!.InsertOneAsync(record);
                return;
            }
            else
            {
                var update = Builders<DbQueueRecord>.Update.Set("RequestedCount", result.RequestedCount+1);
                await _mongoCollection!.UpdateOneAsync(filter, update);
            }
            return;
        }
    }
}
