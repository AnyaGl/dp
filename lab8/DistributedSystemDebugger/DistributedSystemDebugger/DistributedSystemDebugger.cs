using System.Collections.Generic;

namespace DistributedSystemDebugger
{
    public class DistributedSystemDebugger
    {
        private List<int> _time = new List<int>();
        public IEventRecorder CreateEventRecorder(int runId, int processId, string aggreagorUrl)
        {
            while (_time.Count <= processId)
            {
                _time.Add(0);
            }
            return new EventRecorder(runId, processId, aggreagorUrl, IncreaseTime, GetTime);
        }
        private void IncreaseTime(int processId)
        {
            _time[processId]++;
        }
        private List<int> GetTime()
        {
            return _time;
        }
    }
}
