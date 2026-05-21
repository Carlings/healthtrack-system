using HealthTrack.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Common
{
    [ApiController]
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        private ISender? _sender;
        private ICurrentUserService? _currentUser;

        protected ISender Mediator =>
            _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        protected ICurrentUserService CurrentUser =>
            _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

        protected int UserId => CurrentUser.UserId;
        protected string Email => CurrentUser.Email;
        protected bool IsAuthenticated => CurrentUser.IsAuthenticated;
    }
}
