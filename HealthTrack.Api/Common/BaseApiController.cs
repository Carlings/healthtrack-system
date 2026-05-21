using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthTrack.Api.Common
{
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private ISender? _sender;

        protected ISender Mediator => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
