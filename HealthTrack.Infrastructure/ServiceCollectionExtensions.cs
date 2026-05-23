using HealthTrack.Application.Common.Interfaces.Events;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Realtime;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Common.Interfaces.Storage;
using HealthTrack.Infrastructure.Identity;
using HealthTrack.Infrastructure.Persistence;
using HealthTrack.Infrastructure.Realtime;
using HealthTrack.Infrastructure.Repositories;
using HealthTrack.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthTrack.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ITokenValidationService, TokenValidationService>();
        services.AddScoped<IAvatarStorageService, LocalAvatarStorageService>();
        services.AddScoped<IHealthEventsPublisher, HealthEventsPublisher>();
        services.AddScoped<INotificationRealtimePublisher, NotificationRealtimePublisher>();
        services.AddSingleton<INotificationRealtimeBroker, NotificationRealtimeBroker>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IHealthRecordRepository, HealthRecordRepository>();
        services.AddScoped<IGoalRepository, GoalRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}
