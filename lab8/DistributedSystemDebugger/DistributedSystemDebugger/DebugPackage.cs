using System.Collections.Generic;

namespace DistributedSystemDebugger
{
    public class DebugPackage
    {
        public string message;
        public List<int> time;

        public DebugPackage(string msg, List<int> t)
        {
            message = msg;
            time = t;
        }
    }
}
