namespace HealthTrack.Domain.Entities
{
    public class HealthRecord
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime RecordedAt { get; set; }

        public float Weight { get; set; }
        public int Pulse { get; set; }
        public float Temperature { get; set; }
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public int Steps { get; set; }
        public float SleepHours { get; set; }
    }
}
