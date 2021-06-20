using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Aggregator.Models;
using System.Text.Json;

namespace Aggregator.Pages
{
    public class EventsModel : PageModel
    {
        private readonly ILogger<EventsModel> _logger;
        private IStorage _storage;

        [BindProperty]
        public List<int> checkedEvents { get; set; } = new List<int>();

        [BindProperty]
        public int mainEvent { get; set; }
        public List<Event> EventsList { get; set; }
        public EventsModel(ILogger<EventsModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet(int id)
        {
            EventsList = _storage.GetEventsByRunId(id);
        }

        public IActionResult OnPost()
        {
            var json = JsonSerializer.Serialize(checkedEvents);
            string url = Url.Page("CausalRelationship", new {checkedEvents = json, id = mainEvent});
            return Redirect(url);
        }
    }
}
