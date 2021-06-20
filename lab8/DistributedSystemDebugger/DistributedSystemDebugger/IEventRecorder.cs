namespace DistributedSystemDebugger
{
    public interface IEventRecorder
    {
        void DebugInternal(string description);
        DebugPackage DebugSend(string description, string message);
        string DebugReceive(string description, DebugPackage receivedMessage);
    }
}
