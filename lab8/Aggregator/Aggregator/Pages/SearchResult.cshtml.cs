using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Aggregator.Models;

namespace Aggregator.Pages
{
    public class SearchResultModel : PageModel
    {
        private readonly ILogger<SearchResultModel> _logger;
        private IStorage _storage;
        public List<Event> Events { get; set; }
        public int ProcessId { get; set; }
        public int RunId { get; set; }        
        public string TimeVector { get; set; }        
        public string TimeInterval { get; set; }

        public SearchResultModel(ILogger<SearchResultModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet(int processId, int runId, string timeVector, string timeInterval)
        {            
            Events = _storage.GetEventsWithFields(processId, runId, timeVector, timeInterval);
            ProcessId = processId;
            RunId = runId;
            TimeVector = timeVector;
            TimeInterval = timeInterval;
        }
    }
}
