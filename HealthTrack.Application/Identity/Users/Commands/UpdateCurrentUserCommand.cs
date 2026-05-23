using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Users.DTOs;
using MediatR;

namespace HealthTrack.Application.Identity.Users.Commands;

public sealed record UpdateCurrentUserCommand(
    string? Name,
    DateOnly? BirthDate,
    int? Height,
    Domain.Entities.Gender? Gender) : IRequest<CurrentUserDto>;

public sealed class UpdateCurrentUserCommandHandler
    : IRequestHandler<UpdateCurrentUserCommand, CurrentUserDto>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;

    public UpdateCurrentUserCommandHandler(
        IUserRepository users,
        ICurrentUserService currentUser)
    {
        _users = users;
        _currentUser = currentUser;
    }

    public async Task<CurrentUserDto> Handle(
        UpdateCurrentUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetTrackedByIdAsync(
            _currentUser.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", _currentUser.UserId);
        }

        if (request.Name is not null)
        {
            user.Name = request.Name;
        }

        if (request.BirthDate.HasValue)
        {
            user.BirthDate = request.BirthDate;
        }

        if (request.Height.HasValue)
        {
            user.Height = request.Height;
        }

        if (request.Gender.HasValue)
        {
            user.Gender = request.Gender.Value;
        }

        await _users.SaveChangesAsync(cancellationToken);

        DateTime? birthDate = user.BirthDate.HasValue
            ? user.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
            : null;

        return new CurrentUserDto(
            user.Id,
            user.Email,
            user.Name ?? string.Empty,
            birthDate,
            user.Height,
            user.Gender == Domain.Entities.Gender.Unknown
                ? null
                : user.Gender.ToString(),
            user.CreatedAt,
            user.AvatarUrl);
    }
}