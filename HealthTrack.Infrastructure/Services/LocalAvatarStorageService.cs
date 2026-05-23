using HealthTrack.Application.Common.Interfaces.Storage;
using HealthTrack.Application.Common.Options;
using Microsoft.Extensions.Options;

namespace HealthTrack.Infrastructure.Services;

public sealed class LocalAvatarStorageService : IAvatarStorageService
{
    private readonly AvatarStorageOptions _options;

    public LocalAvatarStorageService(IOptions<AvatarStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_options.RootPath);

        var extension = Path.GetExtension(fileName);
        var generatedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_options.RootPath, generatedFileName);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await stream.CopyToAsync(fileStream, cancellationToken);

        return $"{_options.RequestPath}/{generatedFileName}";
    }

    public Task DeleteAsync(
        string avatarUrl,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            return Task.CompletedTask;
        }

        var fileName = Path.GetFileName(avatarUrl);
        var fullPath = Path.Combine(_options.RootPath, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}