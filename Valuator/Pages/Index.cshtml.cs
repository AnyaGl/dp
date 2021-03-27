using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Text;
using System.Threading;
using System.Text.Json;
using LibStorage;
using LibLoggingObjects;

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
        public async Task<IActionResult> OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string similarityKey = Constants.SIMILARITY_PREFIX + id;
            int similarity = GetSimilarity(text, id);
            _storage.Store(similarityKey, similarity.ToString());
            PublishSimilarityCalculatedEvent(id, similarity);

            string textKey = Constants.TEXT_PREFIX + id;
            _storage.StoreText(textKey, text);

            await CalculateAndSaveRank(id);

            return Redirect($"summary?id={id}");
        }
        private async Task CalculateAndSaveRank(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish("valuator.processing.rank", data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
        private void PublishSimilarityCalculatedEvent(string id, int similarity)
        {
            Similarity textSmilarity = new Similarity();
            textSmilarity.TextId = id;
            textSmilarity.Value = similarity;
            
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(textSmilarity));
                c.Publish("valuator.similarity_calculated", data);

                c.Drain();
                c.Close();
            }
        }
        private int GetSimilarity(string text, string id)
        {
            id = Constants.TEXT_PREFIX + id;
            return _storage.IsTextExist(text) ? 1 : 0;
        }
    }
}
