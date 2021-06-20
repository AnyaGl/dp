namespace Aggregator.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public byte Kind { get; set; }
        public int ProcessId { get; set; }
        public int RunId { get; set; }
        public string Time { get; set; }
    }
}
