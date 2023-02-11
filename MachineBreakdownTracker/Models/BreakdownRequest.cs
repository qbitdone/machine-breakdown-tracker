namespace machine_breakdown_tracker.Models
{
    public class BreakdownRequest
    {
        public string Name { get; set; }
        public string Machine { get; set; }
        public string Priority { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public bool Eliminated { get; set; }
    }
}
