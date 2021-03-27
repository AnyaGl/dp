using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace LibStorage
{
    public class RedisStorage : IStorage
    {
        private readonly string host = "localhost";
        private readonly ILogger<RedisStorage> _logger;
        private IConnectionMultiplexer _conn;
        private IConnectionMultiplexer _connRu;
        private IConnectionMultiplexer _connEu;
        private IConnectionMultiplexer _connOther;
        private readonly string _allTextsKey = "allTextsKey";
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _conn = ConnectionMultiplexer.Connect(host);
            _connRu = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_RUS", EnvironmentVariableTarget.User));
            _connEu = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_EU", EnvironmentVariableTarget.User));
            _connOther = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("DB_OTHER", EnvironmentVariableTarget.User));
        }
        public string Load(string shardKey, string key)
        {
            var db = GetConnection(shardKey).GetDatabase();
            return db.KeyExists(key) ? db.StringGet(key) : string.Empty;
        }
        public void Store(string shardKey, string key, string value)
        {
            var db = GetConnection(shardKey).GetDatabase();
            if (!db.StringSet(key, value))
            {
                _logger.LogWarning("Failed to save {0}: {1}", key, value);
            }
        }
        public void StoreText(string shardKey, string key, string text)
        {
            Store(shardKey, key, text);
            StoreTextToSet(shardKey, text);
        }
        public void StoreNewShardKey(string shardKey, string segmentId)
        {
            var db = _conn.GetDatabase();
            if (!db.StringSet(shardKey, segmentId))
            {
                _logger.LogWarning("Failed to save {0}: {1}", shardKey, segmentId);
            }
        }
        public bool IsTextExist(string text)
        {
            var dbRu = _connRu.GetDatabase();
            var dbEu = _connEu.GetDatabase();
            var dbOther = _connOther.GetDatabase();
            return dbRu.SetContains(_allTextsKey, text) || dbEu.SetContains(_allTextsKey, text) || dbOther.SetContains(_allTextsKey, text);
        }
        public string GetSegmentId(string shardKey)
        {
            var db = _conn.GetDatabase();
            return db.KeyExists(shardKey) ? db.StringGet(shardKey) : string.Empty;
        }
        private IConnectionMultiplexer GetConnection(string shardKey)
        {
            var db = _conn.GetDatabase();
            if (!db.KeyExists(shardKey))
            {
                _logger.LogWarning("Shard key \"{0}\" doesn't exist", shardKey);
                return _conn;
            }
            var segmentId = db.StringGet(shardKey);
            switch (segmentId)
            {
                case Constants.SEGMENT_ID_RUS:
                    return _connRu;
                case Constants.SEGMENT_ID_EU:
                    return _connEu;
                case Constants.SEGMENT_ID_OTHER:
                    return _connOther;
                default:
                    _logger.LogWarning("Segment {0} doesn't exist", segmentId);
                    return _conn;
            }
        }
        private void StoreTextToSet(string shardKey, string text)
        {
            var db = GetConnection(shardKey).GetDatabase();
            db.SetAdd(_allTextsKey, text);
        }
    }
}