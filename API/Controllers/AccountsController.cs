using Application.Accounts;
using Application.Orders;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount(CreateAccountCommand command)
        {
            return await Mediator.Send(command);
        } 
        
        [HttpPost("{id}/orders")]
        public async Task<ActionResult<MediatR.Unit>> SendOrder(int id, CreateOrderCommand command)
        {
            if (id < 0)
                return BadRequest(new { id = "AccountId must not be empty." });
                
            command.AccountId = id;
            return await Mediator.Send(command);
        }
    }
}
