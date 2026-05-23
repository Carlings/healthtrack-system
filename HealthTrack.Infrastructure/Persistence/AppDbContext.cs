using HealthTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<HealthRecord> HealthRecords => Set<HealthRecord>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<ActivityType> ActivityTypes => Set<ActivityType>();
        public DbSet<UserActivity> UserActivities => Set<UserActivity>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.Name)
                    .HasMaxLength(200);

                entity.Property(x => x.Gender)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(x => x.CreatedAt)
                    .IsRequired();

                entity.Property(x => x.AvatarUrl)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<HealthRecord>(entity =>
            {
                entity.ToTable("HealthRecords");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.RecordedAt).IsRequired();
                entity.Property(x => x.Weight).HasColumnType("real");
                entity.Property(x => x.Temperature).HasColumnType("real");
                entity.Property(x => x.SleepHours).HasColumnType("real");

                entity.HasIndex(x => new { x.UserId, x.RecordedAt });

                entity.HasOne(x => x.User)
                    .WithMany(x => x.HealthRecords)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Goal>(entity =>
            {
                entity.ToTable("Goals");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.TargetWeight).HasColumnType("real");
                entity.Property(x => x.CreatedAt).IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Goals)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ActivityType>(entity =>
            {
                entity.ToTable("ActivityTypes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.CaloriesPerHour)
                    .HasColumnType("real");

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.ToTable("UserActivities");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.ActivityDate).IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.UserActivities)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ActivityType)
                    .WithMany(x => x.UserActivities)
                    .HasForeignKey(x => x.ActivityTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.UserId, x.ActivityDate });
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Message)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(x => x.Type)
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(x => x.CreatedAt).IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.Notifications)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.UserId, x.IsRead });
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Token)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(x => x.ExpiresAt).IsRequired();
                entity.Property(x => x.CreatedAt).IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.Token)
                    .IsUnique();
            });
        }
    }
}
