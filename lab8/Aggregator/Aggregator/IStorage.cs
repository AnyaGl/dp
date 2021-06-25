using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aggregator.Models;

namespace Aggregator
{
    public interface IStorage
    {
        List<Run> GetRunList();
        List<Event> GetEventsByRunId(int runId);
        CausalRelationship GetCausalRelationship(int currentEventId, List<int> eventsIds);
        List<Event> GetEventsWithFields(int? processId, int runId, string? timeVector, string timeInterval);
    }
}
