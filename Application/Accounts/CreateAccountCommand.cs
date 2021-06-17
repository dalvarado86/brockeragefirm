using Application.Accounts.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Validators;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Accounts
{
    public class CreateAccountCommand : IRequest<AccountResult>
    {
        public decimal Cash { get; set; }
    }

    public class CommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Cash)
                .NotEmpty()
                .GreaterThan(0);
        }
    }

    public class Handler : IRequestHandler<CreateAccountCommand, AccountResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IOptions<MarketSettings> _marketSettings;

        public Handler(IApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IUserAccessor userAccessor,
            IOptions<MarketSettings> marketSettings)
        {
            _context = context;
            _userManager = userManager;
            _userAccessor = userAccessor;
            _marketSettings = marketSettings;
        }

        public async Task<AccountResult> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            if (!BusinessRulesValidator.MarketIsOpen(_marketSettings.Value.TimeOpen, _marketSettings.Value.TimeClose))
            {
                throw new RestException(HttpStatusCode.BadRequest, new { Account = "Market is closed" });
            }

            var account = new Account
            {
                Cash = request.Cash,
                User = user
            };         

            account = _context.Accounts.Add(account).Entity;

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
            {
                return new AccountResult
                {
                    Id = account.Id,
                    Cash = account.Cash
                };
            }

            throw new Exception("There are a problem saving changes");
        }
    }
}
