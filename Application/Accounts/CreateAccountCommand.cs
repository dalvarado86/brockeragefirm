using Application.Accounts.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Validators;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly ILogger<Handler> _logger;

        public Handler(IApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IUserAccessor userAccessor,
            IOptions<MarketSettings> marketSettings,
            ILogger<Handler> logger)
        {
            _context = context;
            _userManager = userManager;
            _userAccessor = userAccessor;
            _marketSettings = marketSettings;
            _logger = logger;
        }

        public async Task<AccountResult> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {           
            if (!BusinessRulesValidator.MarketIsOpen(_marketSettings.Value.TimeOpen, _marketSettings.Value.TimeClose))
            {
                _logger.LogInformation("Market is closed");
                throw new RestException(HttpStatusCode.BadRequest, new { Account = "Market is closed" });
            }

            _logger.LogInformation("Get current username.");
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            var account = new Account
            {
                Cash = request.Cash,
                User = user
            };

            _logger.LogInformation($"Create new account: UserId {account.User.Id}, Amount: {account.Cash}");
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

            _logger.LogError("There are a problem creating the account", nameof(request));
            throw new Common.Exceptions.ApplicationException("There are a problem creating the account");
        }
    }
}
