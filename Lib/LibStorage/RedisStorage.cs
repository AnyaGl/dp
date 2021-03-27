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
        private readonly string _allTextsKey = "allTextsKey";
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _conn = ConnectionMultiplexer.Connect(host);
        }
        public string Load(string key)
        {
            var db = _conn.GetDatabase();
            if (db.KeyExists(key))
            {
                return db.StringGet(key);
            }
            _logger.LogWarning("Key \"{0}\" doesn't exist", key);
            return string.Empty;
        }
        public void Store(string key, string value)
        {
            var db = _conn.GetDatabase();
            if (!db.StringSet(key, value))
            {
                _logger.LogWarning("Failed to save {0}: {1}", key, value);
            }
        }
        public void StoreText(string key, string text)
        {
            Store(key, text);

            var db = _conn.GetDatabase();
            if (!db.SetAdd(_allTextsKey, text))
            {
                _logger.LogWarning("Failed to save {0} to set", text);
            }
        }
        public bool IsTextExist(string text)
        {
            var db = _conn.GetDatabase();
            return db.SetContains(_allTextsKey, text);
        }
    }
}