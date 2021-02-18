using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator
{
    public class RedisStorage : IStorage
    {
        private readonly string host = "localhost";
        private readonly int port = 6379;
        private readonly ILogger<RedisStorage> _logger;
        private IConnectionMultiplexer _conn;
        private List<string> _texts = new List<string>();
        public RedisStorage(ILogger<RedisStorage> logger)
        {
            _logger = logger;
            _conn = ConnectionMultiplexer.Connect(host);
            SaveAllTexts();
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
            else
            {
                _texts.Add(value);
            }
        }

        private void SaveAllTexts()
        {
            var server = _conn.GetServer(host, port);
            foreach (var key in server.Keys(pattern: "TEXT-*"))
            {
                _texts.Add(Load(key));
            }
        }

        public List<string> GetAllTexts()
        {
            return _texts;
        }
    }
}