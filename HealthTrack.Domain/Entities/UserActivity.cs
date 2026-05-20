namespace HealthTrack.Domain.Entities
{
    public class UserActivity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; } = null!;

        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}
