using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users
{
    public class RegisterUserCommand : IRequest<UserResult>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).Password();
        }
    }

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, UserResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtGenerator _jwtGenerator;

        public RegisterUserHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IJwtGenerator jwtGenerator)
        {
            _jwtGenerator = jwtGenerator;
            _userManager = userManager;
            _context = context;
        }

        public async Task<UserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if(await _userManager.FindByEmailAsync(request.Email) != null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists" });

            if (await _userManager.FindByNameAsync(request.UserName) != null)
                throw new RestException(HttpStatusCode.BadRequest, new { UserName = "Username already exists" });

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return new UserResult
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = _jwtGenerator.CreateToken(user),                    
                };
            }

            throw new Exception("Problem creating user");
        }
    }
}
