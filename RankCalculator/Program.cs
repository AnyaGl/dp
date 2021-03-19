using Microsoft.Extensions.Logging;
using LibStorage;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var storage = new RedisStorage(loggerFactory.CreateLogger<RedisStorage>());
            var rankCalculator = new RankCalculator(loggerFactory.CreateLogger<RankCalculator>(), storage);
            rankCalculator.Run();
        }
    }
}