using Microsoft.Extensions.Logging;
using NATS.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LibStorage;
using LibLoggingObjects;

namespace RankCalculator
{
    public class RankCalculator
    {
        ILogger<RankCalculator> _logger;
        private IAsyncSubscription _subscription;
        private IConnection _conn;
        public RankCalculator(ILogger<RankCalculator> logger, IStorage storage)
        {
            _logger = logger;
            _conn = new ConnectionFactory().CreateConnection();

            _subscription = _conn.SubscribeAsync("valuator.processing.rank", "rank_calculator", async (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                _logger.LogDebug("LOOKUP: {0}, {1}", id, storage.GetSegmentId(id));
                
                var text = storage.Load(id, Constants.TEXT_PREFIX + id);
                string rankKey = Constants.RANK_PREFIX + id;
                var rank = GetRank(text);
                storage.Store(id, rankKey, rank.ToString());

                await PublishRankCalculatedEvent(id, rank);
            });
        }
        private async Task PublishRankCalculatedEvent(string id, double rank)
        {
            Rank textRank = new Rank();
            textRank.TextId = id;
            textRank.Value = rank;

            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(textRank));
                c.Publish("rank_calculator.rank_calculated", data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
        public void Run()
        {
            _subscription.Start();

            Console.ReadLine();

            _subscription.Unsubscribe();

            _conn.Drain();
            _conn.Close();
        }

        private double GetRank(string text)
        {
            var nonAlphabeticalCharsCounter = 0;
            foreach (var ch in text)
            {
                if (!Char.IsLetter(ch))
                {
                    nonAlphabeticalCharsCounter++;
                }
            }
            double rank = Convert.ToDouble(nonAlphabeticalCharsCounter) / Convert.ToDouble(text.Length);
            _logger.LogDebug($"Text {text.Substring(0, Math.Min(10, text.Length))} (lenght {text.Length}) contains {nonAlphabeticalCharsCounter} non alphabetical chars and has rank {rank}");

            return rank;
        }

    }
}