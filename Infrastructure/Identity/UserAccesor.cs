using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Infrastructure.Identity
{
    /// <summary>
    /// UserAccesor.
    /// </summary>
    public class UserAccesor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccesor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserAccesor"/> class.
        /// </summary>
        /// <param name="httpContextAccesor">The httpContextAccesor.</param>
        public UserAccesor(IHttpContextAccessor httpContextAccesor)
        {
            _httpContextAccesor = httpContextAccesor;
        }

        /// <inheritdoc/>
        public string GetCurrentUsername()
        {
            var username = _httpContextAccesor.HttpContext.User?.Claims?
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?
                .Value;

            return username;
        }
    }
}
