using System;
using System.Collections.Generic;
using System.Text;

namespace DistributedSystemDebugger
{
    public class Event
    {
        public string Description { get; set; }
        public byte Kind { get; set; }
        public int ProcessId { get; set; }
        public int RunId { get; set; }
        public string Time { get; set; }
    }
}
