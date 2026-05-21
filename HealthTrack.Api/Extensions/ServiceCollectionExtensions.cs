using FluentValidation;
using HealthTrack.Api.Infrastructure;
using HealthTrack.Api.Services;
using HealthTrack.Application.Behaviors;
using HealthTrack.Application.Common.Interfaces;
using MediatR;

namespace HealthTrack.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddValidatorsFromAssembly(typeof(HealthTrack.Application.Common.Interfaces.ICurrentUserService).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(HealthTrack.Application.AssemblyReference).Assembly);
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}