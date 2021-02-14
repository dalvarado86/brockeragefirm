using Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class UserController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserResult>> Login(LoginQuery query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserResult>> Register(RegisterUserCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpGet]
        public async Task<ActionResult<UserResult>> CurrentUser()
        {
            return await Mediator.Send(new CurrentUserQuery());
        }
    }

}
