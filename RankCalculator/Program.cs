using Valuator;
using Microsoft.Extensions.Logging;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var storage = new RedisStorage(new Logger<RedisStorage>(new LoggerFactory()));
            var rankCalculator = new RankCalculator(storage);
            rankCalculator.Run();
        }
    }
}