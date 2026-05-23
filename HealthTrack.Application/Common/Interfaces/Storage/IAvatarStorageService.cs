namespace HealthTrack.Application.Common.Interfaces.Storage;

public interface IAvatarStorageService
{
    Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string avatarUrl,
        CancellationToken cancellationToken);
}