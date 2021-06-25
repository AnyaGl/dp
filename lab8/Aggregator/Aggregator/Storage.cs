using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aggregator.Models;
using System.Text.Json;

namespace Aggregator
{
    public class Storage : IStorage
    {
        private EventsContext _db;
        public Storage(EventsContext db)
        {
            _db = db;
        }
        public List<Run> GetRunList() 
        { 
            var result = _db.Events.GroupBy(e => e.RunId)
                        .Select(g => new Run { Id = g.Key, EventsNumber = g.Count() });

            return result.ToList<Run>();            
        }
        public List<Event> GetEventsByRunId(int runId)
        { 
            var result = _db.Events.Where(e => e.RunId == runId);

            return result.ToList<Event>();  
        }
        public CausalRelationship GetCausalRelationship(int currentEventId, List<int> eventsIds)
        { 
            var cr = new CausalRelationship();
            eventsIds.Remove(currentEventId);
            cr.CurrentEvent = _db.Events.FirstOrDefault(e => e.Id == currentEventId);

            List<int> currEventTime = JsonSerializer.Deserialize<List<int>>(cr.CurrentEvent.Time);
            int processId = cr.CurrentEvent.ProcessId;

            foreach(var id in eventsIds)
            {
                Event e = _db.Events.FirstOrDefault(e => e.Id == id);                
                List<int> eventTime = JsonSerializer.Deserialize<List<int>>(e.Time);

                if (IsGreater(eventTime, currEventTime))
                {
                    cr.AfterEvents.Add(e);
                }
                else if (IsLess(eventTime, currEventTime))
                {                    
                    cr.BeforeEvents.Add(e);
                }
                else if (AreParallel(eventTime, currEventTime))
                {
                    cr.ParallelEvents.Add(e);
                }
            }
            return cr;
        }

        public List<Event> GetEventsWithFields(int? processId, int runId, string? timeVector, string timeInterval)
        {
            var events = _db.Events.ToList();
            events = events.Where(e => e.RunId == runId).ToList();

            if (processId != null)
            {
                events = events.Where(e => e.ProcessId == processId).ToList();
            }
            if (timeVector == null)
            {
                return events;
            }

            Func<List<int>, List<int>, bool> compare;

            switch (timeInterval)
            {
                case "before":
                    compare = IsLess;
                    break;
                case "after":
                    compare = IsGreater;
                    break;
                case "parallel":
                    compare = AreParallel;
                    break;
                case "exactly":
                    compare = AreEqual;
                    break;
                default:
                    throw new Exception($"Unknown time interval: {timeInterval}");
            }

            var resultEvents = new List<Event>();
            var time = JsonSerializer.Deserialize<List<int>>(timeVector);
            foreach(var e in events)
            {
                List<int> eventTime = JsonSerializer.Deserialize<List<int>>(e.Time);
                if (compare(eventTime, time))
                {
                        resultEvents.Add(e);
                }
            }
            return resultEvents;
        }

        private static bool IsGreater(List<int> x, List<int> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            bool greater = false;
            for (int i = 0; i < x.Count; ++i)
            {
                if (x[i] > y[i])
                {
                    greater = true;
                }
                else if (x[i] < y[i])
                {
                    return false;
                }
            }
            return greater;
        }

        private static bool IsLess(List<int> x, List<int> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            bool less = false;
            for (int i = 0; i < x.Count; ++i)
            {
                if (x[i] < y[i])
                {
                    less = true;
                }
                else if (x[i] > y[i])
                {
                    return false;
                }
            }
            return less;
        }

        private static bool AreEqual(List<int> x, List<int> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            for (int i = 0; i < x.Count; ++i)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AreParallel(List<int> x, List<int> y)
        {
            if (x.Count != y.Count)
            {
                return false;
            }
            return !IsLess(x, y) && !IsLess(y, x);
        }
    }
}
