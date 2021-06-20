namespace Aggregator.Models
{
    public class NewEvent
    {
        public string Description { get; set; }
        public byte Kind { get; set; }
        public int ProcessId { get; set; }
        public int RunId { get; set; }
        public string Time { get; set; }
    }
}
