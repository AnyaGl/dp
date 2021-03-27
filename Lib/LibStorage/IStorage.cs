using System.Collections.Generic;

namespace LibStorage
{
    public interface IStorage
    {
        void Store(string shardKey, string key, string value);
        void StoreText(string shardKey, string key, string text);
        void StoreNewShardKey(string chardKey, string segmentId);
        string Load(string shardKey, string key);
        bool IsTextExist(string text);
        string GetSegmentId(string shardKey);
    }
}