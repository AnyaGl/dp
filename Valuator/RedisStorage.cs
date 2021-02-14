using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly ILogger<RedisStorage> _logger;
        private IConnectionMultiplexer _conn;
        
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _conn = ConnectionMultiplexer.Connect("localhost");
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
        public Dictionary<string, string> GetAllValuesWithKeyStartingWith(string keyStart)
        {
            var server = _conn.GetServer("localhost", 6379);

            Dictionary<string, string> values = new Dictionary<string, string>();
            foreach (var key in server.Keys(pattern: keyStart + "*"))
            {
                values.Add(key, Load(key));
            }
            return values;
        }
    }
}