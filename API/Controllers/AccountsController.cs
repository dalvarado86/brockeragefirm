using Application.Accounts;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Account>> Add([FromBody]CreateAccountCommand command)
        {
            return await Mediator.Send(command);
        }       
    }
}
