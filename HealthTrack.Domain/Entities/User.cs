namespace HealthTrack.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string? Name { get; set; }
        public DateOnly? BirthDate { get; set; }
        public int? Height { get; set; }
        public Gender Gender { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<HealthRecord> HealthRecords { get; set; } = new List<HealthRecord>();
        public ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }
}
