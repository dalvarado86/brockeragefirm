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
    /// <summary>
    /// IsAccountHolderRequeriment.
    /// </summary>
    public class IsAccountHolderRequeriment : IAuthorizationRequirement
    {
    }

    /// <summary>
    /// IsAccountHolderRequerimentHandler.
    /// </summary>
    public class IsAccountHolderRequerimentHandler : AuthorizationHandler<IsAccountHolderRequeriment>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="IsAccountHolderRequerimentHandler"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The httpContextAccessor.</param>
        /// <param name="context">The database context.</param>
        public IsAccountHolderRequerimentHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        /// <inheritdoc/>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAccountHolderRequeriment requirement)
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
                    context.Succeed(requirement);
                else
                    context.Fail();
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
