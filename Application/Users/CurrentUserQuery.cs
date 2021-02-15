using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
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

        public CurrentUserHandler(UserManager<ApplicationUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor)
        {
            _userManager = userManager;
            _userAccessor = userAccessor;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<UserResult> Handle(CurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            return new UserResult
            {
                Email = user.Email,
                UserName = user.UserName,
                Token = _jwtGenerator.CreateToken(user)                
            };
        }
    }
}
