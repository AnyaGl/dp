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
    public class CausalRelationshipModel : PageModel
    {
        private readonly ILogger<CausalRelationshipModel> _logger;
        private IStorage _storage;
        public CausalRelationship causalRelationship;

        public CausalRelationshipModel(ILogger<CausalRelationshipModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet(string checkedEvents, int id)
        {
            var events = JsonSerializer.Deserialize<List<int>>(checkedEvents);
            causalRelationship = _storage.GetCausalRelationship(id, events);
        }
    }
}
