using System.Collections.Generic;

namespace LibStorage
{
    public interface IStorage
    {
        void Store(string key, string value);
        void StoreText(string key, string text);
        string Load(string key);
        bool IsTextExist(string text);
    }
}