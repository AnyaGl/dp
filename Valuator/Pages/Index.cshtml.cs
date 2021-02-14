using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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

            string textKey = "TEXT-" + id;
            _storage.Store(textKey, text);

            string rankKey = "RANK-" + id;
            var rank = GetRank(text);
            _storage.Store(rankKey, rank.ToString());

            string similarityKey = "SIMILARITY-" + id;
            int similarity = GetSimilarity(text, id);
            _storage.Store(similarityKey, similarity.ToString());

            return Redirect($"summary?id={id}");
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
        private int GetSimilarity(string text, string id)
        {
            id = "TEXT-" + id;
            var pairs = _storage.GetAllValuesWithKeyStartingWith("TEXT-");
            foreach (var pair in pairs)
            {
                if(pair.Key != id && pair.Value == text)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
