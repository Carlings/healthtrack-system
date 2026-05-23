namespace HealthTrack.Application.Common.Options;

public sealed class AvatarStorageOptions
{
    public const string SectionName = "AvatarStorage";

    public string RootPath { get; set; } = "wwwroot/avatars";
    public string RequestPath { get; set; } = "/avatars";
}