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
    /// <summary>
    /// LoginQuery.
    /// </summary>
    public class LoginQuery : IRequest<UserResult>
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// LoginQueryValidator.
    /// </summary>
    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginQueryValidator"/> class.
        /// </summary>
        public LoginQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    /// <summary>
    /// LoginHandler.
    /// </summary>
    public class LoginHandler : IRequestHandler<LoginQuery, UserResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly ILogger<LoginHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginHandler"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The signin manager.</param>
        /// <param name="jwtGenerator">The jwt generator.</param>
        /// <param name="logger">The logger.</param>
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

        /// <inheritdoc/>
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
