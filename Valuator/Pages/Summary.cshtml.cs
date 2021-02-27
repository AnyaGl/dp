using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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
            _logger.LogDebug(id);
            var counter = 0;
            var rank = _storage.Load(Constants.RANK_PREFIX + id);
            while (rank.Length == 0 && counter < 1000)
            {
                rank = _storage.Load(Constants.RANK_PREFIX + id);
                ++counter;
            }
            if (rank.Length == 0)
            {
                _logger.LogWarning($"rank for id {id} does not found");
            }

            Rank = Convert.ToDouble(rank);            
            Similarity = Convert.ToDouble(_storage.Load(Constants.SIMILARITY_PREFIX + id.ToString()));
        }
    }
}
