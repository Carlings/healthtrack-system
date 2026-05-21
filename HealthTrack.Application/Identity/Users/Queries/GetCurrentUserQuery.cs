using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Identity.Users.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Identity.Users.Queries
{
    public sealed record GetCurrentUserQuery : IRequest<CurrentUserDto>;

    public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
    {
        private readonly IUserRepository _users;
        private readonly ICurrentUserService _currentUser;

        public GetCurrentUserQueryHandler(
            IUserRepository users,
            ICurrentUserService currentUser)
        {
            _users = users;
            _currentUser = currentUser;
        }

        public async Task<CurrentUserDto> Handle(
            GetCurrentUserQuery request,
            CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(_currentUser.UserId, cancellationToken);

            if (user is null)
            {
                throw new NotFoundException("User", _currentUser.UserId);
            }

            DateTime? userBirthDate = user.BirthDate.HasValue
                ? user.BirthDate.Value.ToDateTime(TimeOnly.MinValue)
                : null;

            string? userGender = user.Gender == Gender.Unknown
                ? null
                : user.Gender.ToString();

            return new CurrentUserDto(
                user.Id,
                user.Email,
                user.Name ?? string.Empty,
                userBirthDate,
                user.Height,
                userGender,
                user.CreatedAt);
        }
    }
}
