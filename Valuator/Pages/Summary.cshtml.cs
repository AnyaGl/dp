using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using LibStorage;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug("LOOKUP: {0}, {1}", id, _storage.GetSegmentId(id));
            var counter = 0;
            var rank = _storage.Load(id, Constants.RANK_PREFIX + id);
            while (rank.Length == 0 && counter < 1000)
            {
                rank = _storage.Load(id, Constants.RANK_PREFIX + id);
                ++counter;
            }
            if (rank.Length == 0)
            {
                _logger.LogWarning($"rank for id {id} does not found");
            }

            Rank = Convert.ToDouble(rank);            
            Similarity = Convert.ToDouble(_storage.Load(id, Constants.SIMILARITY_PREFIX + id.ToString()));
        }
    }
}
