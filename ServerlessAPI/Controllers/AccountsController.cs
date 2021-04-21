using Application.Accounts;
using Application.Accounts.Models;
using Application.Orders;
using Application.Orders.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ServerlessAPI.Controllers
{
    public class AccountsController : ApiControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<AccountResult>> CreateAccount(CreateAccountCommand command)
        {
            return await Mediator.Send(command);
        } 
        
        [HttpPost("{id}/orders")]
        [Authorize(Policy = "IsAccountHolder")]
        public async Task<ActionResult<OrderResult>> SendOrder(int id, CreateOrderCommand command)
        {
            if (id < 0)
                return BadRequest(new { id = "AccountId must not be empty." });
                
            command.AccountId = id;
            return await Mediator.Send(command);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "IsAccountHolder")]
        public async Task<ActionResult<AccountDetailsDto>> Detail(int id)
        {            
            return await Mediator.Send(new GetAccountDetailsQuery { AccountId = id });
        }
    }
}
