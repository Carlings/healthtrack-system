using System.Text;
using HealthTrack.Api.Extensions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Options;
using HealthTrack.Infrastructure;
using HealthTrack.Infrastructure.Persistence;
using HealthTrack.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace HealthTrack.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApiServices();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                var section = builder.Configuration.GetSection("Cors:AllowedOrigins");
                var configuredOrigins = section.Get<string[]>() ?? Array.Empty<string>();

                var asString = section.Get<string>();
                if (configuredOrigins.Length == 0 && !string.IsNullOrWhiteSpace(asString))
                {
                    configuredOrigins = asString
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Where(x => x.Length > 0)
                        .ToArray();
                }

                var allowedOrigins = configuredOrigins.Length > 0
                    ? configuredOrigins
                    : new[]
                    {
                        "http://localhost:5173",
                        "http://127.0.0.1:5173"
                    };

                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt");

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings["Key"]!))
                    };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var validator =
                            context.HttpContext.RequestServices
                                .GetRequiredService<ITokenValidationService>();

                        var isValid =
                            await validator.IsValidAsync(
                                context.Principal!,
                                context.HttpContext.RequestAborted);

                        if (!isValid)
                        {
                            context.Fail("Token revoked.");
                        }
                    }
                };
            });

        builder.Services.Configure<AvatarStorageOptions>(
            builder.Configuration.GetSection(
                AvatarStorageOptions.SectionName));

        builder.Services.PostConfigure<AvatarStorageOptions>(options =>
        {
            if (!Path.IsPathRooted(options.RootPath))
            {
                options.RootPath =
                    Path.GetFullPath(
                        Path.Combine(
                            builder.Environment.ContentRootPath,
                            options.RootPath));
            }
        });

        builder.Services.AddAuthorization();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HealthTrack.Api",
                Version = "v1"
            });

            var jwtSecurityScheme =
                new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Enter JWT Bearer token only",

                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

            options.AddSecurityDefinition(
                "Bearer",
                jwtSecurityScheme);

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
        });

        var app = builder.Build();

        app.UseExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("Frontend");

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db =
                scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

            var passwordHasherService =
                scope.ServiceProvider
                    .GetRequiredService<IPasswordHasherService>();

            await AppDbSeeder.SeedAsync(
                db,
                passwordHasherService);
        }

        await app.RunAsync();
    }
}
