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

                if (eventTime[processId] > currEventTime[processId])
                {
                    cr.AfterEvents.Add(e);
                }
                else if (eventTime[processId] < currEventTime[processId])
                {                    
                    cr.BeforeEvents.Add(e);
                }
                else
                {
                    cr.ParallelEvents.Add(e);
                }
            }
            return cr;
        }

        public List<Event> GetEventsWithFields(int processId, int runId, string timeVector, string timeInterval)
        {
            var events = _db.Events.ToList();
            events = events.Where(e => e.RunId == runId).ToList();
            events = events.Where(e => e.ProcessId == processId).ToList();


            var resultEvents = new List<Event>();

            if (timeInterval == "exactly")
            {
                foreach(var e in events)
                {
                    if (e.Time == timeVector)
                    {
                        resultEvents.Add(e);
                    }
                }
            }
            
            var time = JsonSerializer.Deserialize<List<int>>(timeVector);
            if (timeInterval == "before")
            {
                foreach(var e in events)
                {
                    List<int> eventTime = JsonSerializer.Deserialize<List<int>>(e.Time);
                    if (eventTime[processId] < time[processId])
                    {
                        resultEvents.Add(e);
                    }
                }
            }

            if (timeInterval == "after")
            {
                foreach(var e in events)
                {
                    List<int> eventTime = JsonSerializer.Deserialize<List<int>>(e.Time);
                    if (eventTime[processId] > time[processId])
                    {
                        resultEvents.Add(e);
                    }
                }
            }

            if (timeInterval == "parallel")
            {
                foreach(var e in events)
                {
                    List<int> eventTime = JsonSerializer.Deserialize<List<int>>(e.Time);
                    if (eventTime[processId] == time[processId])
                    {
                        resultEvents.Add(e);
                    }
                }
            }

            return resultEvents;
        }
    }
}
