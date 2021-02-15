using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Security
{
    public class IsAccountHolderRequeriment : IAuthorizationRequirement
    {
    }

    public class IsAccountHolderRequerimentHandler : AuthorizationHandler<IsAccountHolderRequeriment>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public IsAccountHolderRequerimentHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext authContext, IsAccountHolderRequeriment requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext.Request.RouteValues.ContainsKey("id"))
            {
                var currentUserName = _httpContextAccessor.HttpContext.User?.Claims?
                    .SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                var accountId = Convert.ToInt32(httpContext.Request.RouteValues["id"]);
                var account = _context.Accounts
                    .Include(x => x.User)
                    .SingleOrDefaultAsync(x => x.Id == accountId).Result;

                if (account.User.UserName == currentUserName)
                    authContext.Succeed(requirement);
                else
                    authContext.Fail();
            }
            else
            {
                authContext.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
