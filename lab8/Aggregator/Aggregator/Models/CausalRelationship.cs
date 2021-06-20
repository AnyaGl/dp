using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aggregator.Models
{
    public class CausalRelationship
    {
        public Event CurrentEvent { get; set; }
        public List<Event> BeforeEvents { get; set; } = new List<Event>();
        public List<Event> AfterEvents { get; set; } = new List<Event>();
        public List<Event> ParallelEvents { get; set; } = new List<Event>();
    }
}
