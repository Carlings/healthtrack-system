namespace HealthTrack.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public enum NotificationType
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }
}
