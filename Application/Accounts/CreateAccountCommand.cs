using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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

        public Handler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserAccessor userAccessor)
        {
            _context = context;
            _userManager = userManager;
            _userAccessor = userAccessor;
        }

        public async Task<AccountResult> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

            var account = new Account
            {
                Cash = request.Cash,
                User = user
            };         

            account = _context.Accounts.Add(account).Entity;

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (success)
                return new AccountResult
                {
                    Id = account.Id,
                    Cash = account.Cash
                };

            throw new Exception("There are a problem saving changes");
        }
    }

    public class AccountResult
    {
        public int Id { get; set; }
        public decimal Cash { get; set; }
        public IList<string> Issuers { get; private set; } = new List<string>();
    }
}
