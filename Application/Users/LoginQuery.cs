using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class LoginQuery : IRequest<UserResult>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class LoginHandler : IRequestHandler<LoginQuery, UserResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IJwtGenerator jwtGenerator,
            ILogger<LoginHandler> logger)
        {
            _jwtGenerator = jwtGenerator;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<UserResult> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Get user by email '{request.Email}'");
            var user = await _userManager.FindByEmailAsync(request.Email);           

            if (user == null)
            {
                _logger.LogInformation($"User not found.");
                throw new RestException(HttpStatusCode.Unauthorized);
            }

            _logger.LogInformation($"Validate user credentials.");
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                _logger.LogInformation($"The authentication was successful");
                return new UserResult
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _jwtGenerator.CreateToken(user)
                };
            }

            _logger.LogError("The user credentials are not valid.");
            throw new RestException(HttpStatusCode.Unauthorized);
        }
    }
}
