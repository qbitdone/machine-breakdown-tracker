namespace machine_breakdown_tracker.Models
{
    public class Machine
    {
        public string Name { get; set; }
        public List<Breakdown> Breakdowns { get; set; }
    }
}
