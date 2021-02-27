using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Text;
using System.Threading;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }
        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = Constants.SIMILARITY_PREFIX + id;
            int similarity = GetSimilarity(text, id);
            _storage.Store(similarityKey, similarity.ToString());

            string textKey = Constants.TEXT_PREFIX + id;
            _storage.Store(textKey, text);

            CalculateAndSaveRank(id);

            return Redirect($"summary?id={id}");
        }
        private void CalculateAndSaveRank(string id)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task.Factory.StartNew(() => ProduceAsync(id, cts.Token), cts.Token);
        }

        private async Task ProduceAsync(string id, CancellationToken ct)
        {
            ConnectionFactory cf = new ConnectionFactory();

            using (IConnection c = cf.CreateConnection())
            {
                if (!ct.IsCancellationRequested)
                {
                    byte[] data = Encoding.UTF8.GetBytes(id);
                    c.Publish("valuator.processing.rank", data);
                    await Task.Delay(1000);
                }
                
                c.Drain();
                c.Close();
            }
        }
        private int GetSimilarity(string text, string id)
        {
            id = Constants.TEXT_PREFIX + id;
            var values = _storage.GetAllTexts();
            foreach (var value in values)
            {
                if(value == text)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
