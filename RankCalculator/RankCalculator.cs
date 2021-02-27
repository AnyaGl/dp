using NATS.Client;
using System;
using System.Text;
using System.Threading.Tasks;
using Valuator;

namespace RankCalculator
{
    public class RankCalculator
    {
        private IAsyncSubscription _subscription;
        IConnection _conn;
        public RankCalculator(IStorage storage)
        {
            _conn = new ConnectionFactory().CreateConnection();

            _subscription = _conn.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                var text = storage.Load("TEXT-" + id);
                string rankKey = "RANK-" + id;
                var rank = GetRank(text);
                storage.Store(rankKey, rank.ToString());
            });
        }

        public void Run()
        {
            _subscription.Start();

            Console.ReadLine();        
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
            return Convert.ToDouble(nonAlphabeticalCharsCounter) / Convert.ToDouble(text.Length);
        }

        ~RankCalculator()
        {

            _subscription.Unsubscribe();

            _conn.Drain();
            _conn.Close();  

        }

    }
}