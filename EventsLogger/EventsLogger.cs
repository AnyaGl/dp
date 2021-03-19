using System;
using System.Text;
using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibLoggingObjects;

namespace EventsLogger
{
    public class EventsLogger
    {
        private ILogger<EventsLogger> _logger;
        private IAsyncSubscription _subscription;
        private IConnection _conn;
        
        public EventsLogger(ILogger<EventsLogger> logger)
        {
            _logger = logger;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _conn = new ConnectionFactory().CreateConnection();

            _subscription = _conn.SubscribeAsync("rank_calculator.rank_calculated", (sender, args) =>
            {
                Rank rank = JsonSerializer.Deserialize<Rank>(args.Message.Data);
                _logger.LogDebug($"Event: {args.Message.Subject}\nText with id {rank.TextId} has rank {rank.Value}"); 
            });
            
            _subscription = _conn.SubscribeAsync("valuator.similarity_calculated", (sender, args) =>
            {
                Similarity similarity = JsonSerializer.Deserialize<Similarity>(args.Message.Data);                
                _logger.LogDebug($"Event: {args.Message.Subject}\nText with id {similarity.TextId} has similarity {similarity.Value}"); 
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
    }
}