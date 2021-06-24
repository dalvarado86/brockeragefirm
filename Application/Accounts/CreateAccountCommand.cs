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
    /// <summary>
    /// CreateAccountCommand.
    /// </summary>
    public class CreateAccountCommand : IRequest<AccountResult>
    {
        /// <summary>
        /// Gets or sets the cash amount.
        /// </summary>
        public decimal Cash { get; set; }
    }

    /// <summary>
    /// CommandValidator.
    /// </summary>
    public class CommandValidator : AbstractValidator<CreateAccountCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidator"/> class.
        /// </summary>
        public CommandValidator()
        {
            RuleFor(x => x.Cash)
                .NotEmpty()
                .GreaterThan(0);
        }
    }

    /// <summary>
    /// Handler.
    /// </summary>
    public class Handler : IRequestHandler<CreateAccountCommand, AccountResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IOptions<MarketSettings> _marketSettings;
        private readonly ILogger<Handler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Handler"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userAccessor">The user accesor.</param>
        /// <param name="marketSettings">The market settings</param>
        /// <param name="logger">The logger.</param>
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

        /// <inheritdoc/>
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
