using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace Aggregator.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ILogger<SearchModel> _logger;
        private IStorage _storage;
        public List<SelectListItem> Options { get; set; } = new List<SelectListItem> {
                                    new SelectListItem { Value = "before", Text = "before" },
                                    new SelectListItem { Value = "after", Text = "after" },
                                    new SelectListItem { Value = "parallel", Text = "parallel" },
                                    new SelectListItem { Value = "exactly", Text = "exactly" },
                                  };

        public SearchModel(ILogger<SearchModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var processId = Request.Form["ProcessId"];
            var runId = Request.Form["RunId"];
            var timeVector = Request.Form["TimeVector"];
            var timeInterval = Request.Form["Time"];

            string url = Url.Page("SearchResult", new {processId = Int32.Parse(processId), runId = Int32.Parse(runId), timeVector = timeVector, timeInterval = timeInterval});
            return Redirect(url);
        }
    }
}
