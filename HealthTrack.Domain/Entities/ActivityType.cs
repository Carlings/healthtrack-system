namespace HealthTrack.Domain.Entities
{
    public class ActivityType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public float CaloriesPerHour { get; set; }

        public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
    }
}
