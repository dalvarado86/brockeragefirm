using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class CurrentUserQuery : IRequest<UserResult>
    {
    }

    public class CurrentUserHandler : IRequestHandler<CurrentUserQuery, UserResult>
    {
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CurrentUserHandler> _logger;

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
