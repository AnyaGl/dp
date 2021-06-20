using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aggregator.Models;

namespace Aggregator.Controllers
{
    [ApiController]
    [Route("api")]
    public class EventsController : Controller
    {
        EventsContext db;
        public EventsController(EventsContext context)
        {
            db = context;
        }

        [HttpPost]
        public async Task<ActionResult<Event>> Post(NewEvent newEvent)
        {
            if (newEvent == null)
            {
                return BadRequest();
            }

            var e = new Event()
            {
                Description = newEvent.Description,
                Kind = newEvent.Kind,
                ProcessId = newEvent.ProcessId,
                RunId = newEvent.RunId,
                Time = newEvent.Time
            };

            db.Events.Add(e);
            await db.SaveChangesAsync();
            return Ok(e);
        }
    }
}
