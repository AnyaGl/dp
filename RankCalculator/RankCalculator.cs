using Microsoft.Extensions.Logging;
using NATS.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Valuator;

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

            _subscription = _conn.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                var text = storage.Load(Constants.TEXT_PREFIX + id);
                string rankKey = Constants.RANK_PREFIX + id;
                var rank = GetRank(text);
                storage.Store(rankKey, rank.ToString());
            });
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
            double rank =  Convert.ToDouble(nonAlphabeticalCharsCounter) / Convert.ToDouble(text.Length);
            _logger.LogDebug($"Text {text.Substring(0, Math.Min(10, text.Length))} (lenght {text.Length}) contains {nonAlphabeticalCharsCounter} non alphabetical chars and has rank {rank}");

            return rank;
        }

    }
}