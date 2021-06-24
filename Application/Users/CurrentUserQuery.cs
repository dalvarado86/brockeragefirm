using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    /// <summary>
    /// CurrentUserQuery.
    /// </summary>
    public class CurrentUserQuery : IRequest<UserResult>
    {
    }

    /// <summary>
    /// CurrentUserHandler.
    /// </summary>
    public class CurrentUserHandler : IRequestHandler<CurrentUserQuery, UserResult>
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CurrentUserHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserHandler"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="jwtGenerator">The jwt generator.</param>
        /// <param name="userAccessor">The user accessor.</param>
        /// <param name="logger">The logger.</param>
        public CurrentUserHandler(
            UserManager<ApplicationUser> userManager, 
            IJwtGenerator jwtGenerator, 
            IUserAccessor userAccessor,
            ILogger<CurrentUserHandler> logger)
        {
            _userManager = userManager;
            _userAccessor = userAccessor;
            _jwtGenerator = jwtGenerator;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<UserResult> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            _logger.LogInformation($"User retrieved: '{user.UserName}'");

            return new UserResult
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = _jwtGenerator.CreateToken(user)                
            };
        }
    }
}
