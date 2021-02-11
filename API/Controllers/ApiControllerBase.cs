using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator =>
            _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}
